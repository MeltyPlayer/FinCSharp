using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;

using fin.assert;
using fin.emulation.gb.memory;
using fin.emulation.gb.memory.io;
using fin.graphics.color;
using fin.log;

using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

using SharpFont.PostScript;

namespace fin.emulation.gb {
  public class Cpu {
    private readonly Lcd lcd_;
    private readonly Mmu memory_;
    private readonly IOpcodes opcodes_;

    private IMemoryMap MemoryMap { get; }
    private IoAddresses IoAddresses { get; }

    public Cpu(Lcd lcd, Mmu memory, IOpcodes opcodes) {
      this.lcd_ = lcd;
      this.memory_ = memory;
      this.opcodes_ = opcodes;

      this.MemoryMap = this.memory_.MemoryMap;
      this.IoAddresses = this.MemoryMap.IoAddresses;

      this.Reset();
    }

    public void Reset() {
      this.UpwardScanlineCycleCounter = 0; //456 - 288; // 288 // 456;
      this.PpuModeCycleCount = 80;
    }

    public int ExecuteCycles(int maxCycles)
      => this.Execute_(maxCycles, null);

    public int ExecuteInstructions(int maxInstructions)
      => this.Execute_(null, maxInstructions);

    private int Execute_(int? maxCycles, int? maxInstructions) {
      var totalCycles = 0;
      var totalIterations = 0;

      var memory = this.memory_;
      var stack = memory.Stack;
      var pc = memory.Registers.Pc;

      var ioAddresses = this.IoAddresses;
      var if_ = ioAddresses.If;

      for (;;) {
        var cyclesThisIteration = 0;

        var ieV = ioAddresses.Ie.Value;
        var ifV = if_.Value;
        // TODO: Handle interrupts
        var ieAndIf = ieV & ifV;
        if (ieAndIf != 0) {
          memory.HaltState = HaltState.NOT_HALTED;

          if (memory.InterruptsState == InterruptsState.ON) {
            //v-sync
            if ((ieAndIf & 0x1) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              if_.Value = (byte) (ifV & ~0x1);

              stack.Push16(pc.Value);
              pc.Value = 0x40;
            }
            //LCD STAT
            else if ((ieAndIf & 0x2) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              if_.Value = (byte) (ifV & ~0x2);

              stack.Push16(pc.Value);
              pc.Value = 0x48;
            }
            //Timer
            else if ((ieAndIf & 0x4) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              if_.Value = (byte) (ifV & ~0x4);

              stack.Push16(pc.Value);
              pc.Value = 0x50;
            }
            //Serial
            else if ((ieAndIf & 0x8) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              if_.Value = (byte) (ifV & ~0x8);

              stack.Push16(pc.Value);
              pc.Value = 0x58;
            }
            //Joypad
            else if ((ieAndIf & 0x10) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              if_.Value = (byte) (ifV & ~0x10);

              stack.Push16(pc.Value);
              pc.Value = 0x60;
            }
          }
        }


        // Enables interrupts if scheduled by EI
        if (memory.InterruptsState == InterruptsState.SCHEDULED_TO_BE_ON) {
          memory.InterruptsState = InterruptsState.ON;
        }


        // Runs instruction if not halted
        if (memory.HaltState != HaltState.HALTED) {
          var cycles = this.opcodes_.FetchAndRunOp();
          cyclesThisIteration += cycles;
        } else {
          cyclesThisIteration += 4;
        }


        // Updates timers, LCD w/ usedCycles
        this.UpdateTimers_(cyclesThisIteration);
        this.UpdateLcdStatus_(cyclesThisIteration);

        // Checks if we're out of cycles
        totalCycles += cyclesThisIteration;
        ++totalIterations;

        if (totalCycles >= (maxCycles ?? totalCycles + 1)) {
          return totalCycles;
        }
        if (totalIterations >= (maxInstructions ?? totalIterations + 1)) {
          return totalCycles;
        }
      }
    }

    private readonly int[] tacCycles_ = {
        1024, 16, 64, 256,
    };

    private void UpdateTimers_(int cyclesThisIteration) {
      var ioAddresses = this.IoAddresses;

      //DIV
      ioAddresses.DivCounter += cyclesThisIteration;

      if (ioAddresses.DivCounter >= 256) {
        ioAddresses.DivCounter -= 256;
        ++ioAddresses.Div.Value;
      }

      //TIMA
      if ((ioAddresses.Tac.Value & 0x4) != 0) {
        ioAddresses.TimerCounter += cyclesThisIteration;

        var tacCycles = this.tacCycles_[ioAddresses.Tac.Value & 0x3];
        while (ioAddresses.TimerCounter >= tacCycles) {
          ioAddresses.TimerCounter -= tacCycles;

          //overflow
          if (++ioAddresses.Tima.Value == 0) {
            this.InterruptZ80(InterruptType.TIMER);
            ioAddresses.Tima.Value = ioAddresses.Tma.Value;
          }
        }
      }
    }

    public int ScanlineCycleCounter => 456 - this.UpwardScanlineCycleCounter;
    public int UpwardScanlineCycleCounter { get; set; }
    public int PpuMode => (int) this.IoAddresses.Stat.Mode;

    public int PpuModeCycleCount { get; set; }

    private void UpdateLcdStatus_(int cyclesThisIteration) {
      var ioAddresses = this.IoAddresses;
      var stat = ioAddresses.Stat;

      // TODO: What does this do?
      stat.Value |= 0x80;

      var ly = ioAddresses.Ly.Value;
      var lcdc = ioAddresses.Lcdc.Value;

      //lcd enabled
      if ((lcdc & 0x80) != 0) {
        for (var ci = 0; ci < cyclesThisIteration; ci += 4) {
          this.UpwardScanlineCycleCounter += 4;

          var mode = stat.Mode;
          if (mode == PpuModeType.OAM_RAM_SEARCH) {
            if (this.UpwardScanlineCycleCounter > 80) {
              this.ChangeStat_(PpuModeType.DATA_TRANSFER);
              this.PpuModeCycleCount = this.GetMode3TickCount();
            }
          }
          // TODO: Dependent on number of sprites on this line
          else if (mode == PpuModeType.DATA_TRANSFER) {
            if (this.UpwardScanlineCycleCounter >
                80 + this.PpuModeCycleCount) {
              this.ChangeStat_(PpuModeType.HBLANK);
              this.PpuModeCycleCount = 456 - this.UpwardScanlineCycleCounter;

              this.DrawScanline_();
            }
          } else if (mode == PpuModeType.HBLANK) {
            // 80 + 172 + 204
            if (this.UpwardScanlineCycleCounter > 456) {
              this.UpwardScanlineCycleCounter -= 456;

              //starting vsync period
              if (++ly >= 144) {
                this.ChangeStat_(PpuModeType.VBLANK);
                this.InterruptZ80(InterruptType.V_BLANK);
              } else {
                this.ChangeStat_(PpuModeType.OAM_RAM_SEARCH);
                this.PpuModeCycleCount = 80;
              }
              ioAddresses.Ly.Value = ly;
            }
            // Needed for the first frame???
            else if (this.UpwardScanlineCycleCounter < 80 + 172 &&
                     this.UpwardScanlineCycleCounter > 80) {
              this.ChangeStat_(PpuModeType.DATA_TRANSFER);
              this.PpuModeCycleCount = 172;
            }
          } else if (mode == PpuModeType.VBLANK) {
            if (this.UpwardScanlineCycleCounter > 456) {
              this.UpwardScanlineCycleCounter -= 456;
              if (++ly >= 154) {
                ly = 0;
                this.ChangeStat_(PpuModeType.OAM_RAM_SEARCH);
              }
              ioAddresses.Ly.Value = ly;
            }
          }

          this.LyLyc_(ly);
        }
      } else {
        // Reset
        stat.Mode = PpuModeType.HBLANK;
        this.UpwardScanlineCycleCounter = 0;
        ioAddresses.Ly.Value = 0;
      }
    }

    public delegate void OnEnterVblankHandler();

    public event OnEnterVblankHandler OnEnterVblank;

    private void ChangeStat_(PpuModeType newModeType) {
      var stat = this.IoAddresses.Stat;
      stat.Mode = newModeType;

      var shouldTriggerLcdInterrupt = newModeType switch {
          PpuModeType.OAM_RAM_SEARCH => stat.OamRamSearchInterruptEnabled,
          PpuModeType.HBLANK        => stat.HblankInterruptEnabled,
          PpuModeType.VBLANK        => stat.VblankInterruptEnabled,
          _                          => false,
      };

      if (newModeType == PpuModeType.VBLANK) {
        this.OnEnterVblank?.Invoke();
      }

      if (shouldTriggerLcdInterrupt) {
        this.InterruptZ80(InterruptType.LCD_STAT);
      }
    }


    private class Obj {
      public int X { get; }
      public int Y { get; }

      public Obj(int x, int y) {
        this.X = x;
        this.Y = y;
      }
    }

    private int GetMode3TickCount() {
      var memoryMap = this.MemoryMap;
      var ioAddresses = this.IoAddresses;

      var minTicks = 172;

      // Obj display
      var lcdc = ioAddresses.Lcdc.Value;
      if ((lcdc & 0x1) == 0) {
        return minTicks;
      }

      var objSize = (lcdc >> 2) & 0x1; // Bit 3 (index = 2)
      var objHeight = objSize switch {
          0 => 8,
          1 => 16,
          _ => Asserts.Fail("Invalid obj size") ? 0 : 0,
      };

      var ly = ioAddresses.Ly.Value;

      var lineObjCount = 0;

      var maxObjsPerLine = 10;
      var objs = new LinkedList<Obj>();

      var oam = memoryMap.Oam;

      var objCount = 40;
      for (var i = 0; i < objCount; ++i) {
        /* Put the visible sprites into line_obj. Insert them so sprites with
         * smaller X-coordinates are earlier, but only on DMG. On CGB, they are
         * always ordered by obj index. */

        // Sprite attrib memory (OAM) address
        var oamAddress = (ushort) (i * 4);
        var objY = oam[oamAddress] - 16;

        var relY = ly - objY;

        if (relY < objHeight) {
          var objX = oam[(ushort) (oamAddress + 1)] - 8;

          objs.AddLast(new Obj(objX, objY));

          /*int j = line_obj_count;
          if (!IS_CGB) {
            while (j > 0 && o->x < PPU.line_obj[j - 1].x) {
              PPU.line_obj[j] = PPU.line_obj[j - 1];
              j--;
            }
          }
          PPU.line_obj[j] = *o;*/
          if (++lineObjCount == maxObjsPerLine) {
            break;
          }
        }
      }

      var screenWidth = 160;
      var bucketCount = screenWidth / 8 + 2;
      var buckets = new int[bucketCount];

      var scX = ioAddresses.ScrollX.Value;
      var scxFine = scX & 7;

      var ticks = minTicks + scxFine;
      var hasZero = false;

      foreach (var obj in objs) {
        var x = obj.X + 8;
        if (x >= screenWidth + 8) {
          continue;
        }

        if (!hasZero && x == 0) {
          hasZero = true;
          ticks += scxFine;
        }

        x += scxFine;
        var bucket = x >> 3;
        buckets[bucket] = Math.Max(buckets[bucket], 5 - (x & 7));
        ticks += 6;
      }
      foreach (var t in buckets) {
        ticks += t;
      }
      return ticks;
    }

    // TODO: This needs to be moved to IoAddresses, so it can be checked the
    // instant they become equal.
    private void LyLyc_(byte ly) {
      var ioAddresses = this.IoAddresses;
      var stat = ioAddresses.Stat;

      //LY=LYC?
      if (ly == ioAddresses.Lyc.Value) {
        //STAT
        ioAddresses.Stat.Value |= 0x4;
        if ((stat.Value & 0x40) != 0) {
          this.InterruptZ80(InterruptType.LCD_STAT);
        }
      } else {
        stat.Value = (byte) (stat.Value & ~0x4);
      }
    }

    public enum InterruptType {
      V_BLANK,
      LCD_STAT,
      TIMER,
      SERIAL,
      JOYPAD,
    }

    public void InterruptZ80(InterruptType type) {
      var if_ = this.IoAddresses.If;
      switch (type) {
        case InterruptType.V_BLANK:
          if_.Value |= 0x1;
          break;
        case InterruptType.LCD_STAT:
          if_.Value |= 0x2;
          break;
        case InterruptType.TIMER:
          if_.Value |= 0x4;
          break;
        case InterruptType.SERIAL:
          if_.Value |= 0x8;
          break;
        case InterruptType.JOYPAD:
          if_.Value |= 0x10;
          break;
      }
    }

    public byte ScanlineLcdc { get; private set; } = 0;

    private void DrawScanline_() {
      if (!this.lcd_.Active) {
        return;
      }

      var lcdc = this.IoAddresses.Lcdc.Value;
      this.ScanlineLcdc = lcdc;

      if ((lcdc & 0x1) != 0) {
        this.DrawBg_();
      }

      if ((lcdc & 0x2) != 0) {
        this.DrawSprites_();
      }
    }

    private void DrawBg_() {
      var memoryMap = this.MemoryMap;
      var ioAddresses = this.IoAddresses;

      var lcdc = ioAddresses.Lcdc.Value;
      var wy = ioAddresses.Wy.Value;
      var ly = ioAddresses.Ly.Value;

      //window enabled and scanline within window ?
      var useWindow = (lcdc & (1 << 5)) != 0 && wy <= ly;

      byte y;
      int backgroundAddress;
      if (useWindow) {
        y = (byte) (ly - wy);

        //Window Tile Map Display Select
        backgroundAddress = ((lcdc & (1 << 6)) != 0)
                                //0x9c00
                                ? 0x1C00
                                //0x9800
                                : 0x1800;
      }
      //not using window
      else {
        y = (byte) (ioAddresses.ScrollY.Value + ly);

        //Window Tile Map Display Select
        backgroundAddress = ((lcdc & (1 << 3)) != 0)
                                //0x9c00
                                ? 0x1C00
                                //0x9800
                                : 0x1800;
      }


      // each vertical line takes up two bytes of memory
      var tileLine = (byte) ((y & 7) * 2);

      //TODO: testing divide by 8 == multiply by 0.125
      //rowPos o current scanline (of the 8 pixels)
      var tileRow = (ushort) (y / 8 * 32);

      var scX = ioAddresses.ScrollX.Value;
      var wX = (byte) (ioAddresses.Wx.Value - 7);

      var areTileAddressesSigned = (lcdc & (1 << 4)) == 0;

      byte upper = 0;
      byte lower = 0;

      var bgp = ioAddresses.Bgp.Value;

      var vram = memoryMap.Vram;

      //draw de 160 pixels in current line  TODO: (4 by 4)
      for (var p = 0; p < 160; p++) {
        var x = useWindow && p > wX ? (byte) (p - wX) : (byte) (p + scX);

        if ((p & 7) == 0 || ((p + scX) & 7) == 0) {
          var tileCol = (ushort) (x / 8);
          var tileNumber =
              vram[(ushort) (backgroundAddress + tileRow + tileCol)];

          ushort tileAddress;
          if (!areTileAddressesSigned) {
            tileAddress = (ushort) (0x000 + tileNumber * 16);
          } else {
            tileAddress =
                (ushort) (0x800 +
                          (128 + ByteMath.ByteToSByte(tileNumber)) * 16);
          }

          var vramAddress = (ushort) (tileAddress + tileLine);
          lower = vram[vramAddress];
          upper = vram[(ushort) (vramAddress + 1)];
        }

        var colorBit = (byte) (7 - (x & 7));
        var colorId = this.GetColorId_(colorBit, lower, upper);
        var color = this.GetColor_(bgp, colorId);

        //draw
        this.lcd_.SetPixel(p, ly, color);
      }
    }

    private void DrawSprites_() {
      var memoryMap = this.MemoryMap;
      var vram = memoryMap.Vram;
      var oam = memoryMap.Oam;
      var ioAddresses = this.IoAddresses;

      var ly = ioAddresses.Ly.Value;
      var lcdc = ioAddresses.Lcdc.Value;

      var obp0 = ioAddresses.Obp0.Value;
      var obp1 = ioAddresses.Obp1.Value;

      var notOnLineColor = Color.FromCColor(System.Drawing.Color.DimGray);
      var transparentColor = Color.FromCColor(System.Drawing.Color.Aqua);
      var offscreenColor = Color.FromCColor(System.Drawing.Color.Yellow);
      var notOverBgColor = Color.FromCColor(System.Drawing.Color.Red);
      var bgNotWhiteColor = Color.FromCColor(System.Drawing.Color.Green);

      //loop throught the 40 sprites
      for (var i = 0; i < 40; i++) {
        Color? didNotRenderColor = null;

        var spriteAddress = (ushort) (i * 4);
        var y = (byte) (oam[spriteAddress] - 16);
        var x = (byte) (oam[(ushort) (spriteAddress + 1)] - 8);
        var tile = oam[(ushort) (spriteAddress + 2)];
        var attributes = oam[(ushort) (spriteAddress + 3)];

        //check y - Size in LCDC
        var sizeY = BitMath.GetBit(lcdc, 2) ? 16 : 8;

        //check if sprite is in current Scanline
        if (ly >= y && ly < y + sizeY) {
          var isXFlipped = (attributes & 0x20) != 0;
          var isYFlipped = (attributes & 0x40) != 0;
          var isOverBg = (attributes >> 7) == 0;
          var palette = BitMath.GetBit(attributes, 4) ? obp1 : obp0;

          var tileRow = isYFlipped ? sizeY - 1 - (ly - y) : ly - y;

          var tileAddress = (ushort) ((tile * 16) + (tileRow * 2));
          var lower = vram[tileAddress];
          var upper = vram[(ushort) (tileAddress + 1)];

          for (var p = 0; p < 8; p++) {
            var colorBit = (byte) (isXFlipped ? p : 7 - p);
            var colorId = this.GetColorId_(colorBit, lower, upper);

            // White is transparent for sprites.
            if (colorId == 0) {
              didNotRenderColor = transparentColor;
              continue;
            }

            var sX = x + p;
            if (sX < 0 || sX >= 160) {
              didNotRenderColor = offscreenColor;
              continue;
            }

            var color = this.GetColor_(palette, colorId);

            var isBgWhite = this.lcd_.GetPixel(sX, ly).Rb == 255;
            if (isOverBg || isBgWhite) {
              this.lcd_.SetPixel(sX, ly, color);
            } else {
              if (!isOverBg) {
                didNotRenderColor = notOverBgColor;
              } else {
                didNotRenderColor = bgNotWhiteColor;
              }
            }
          }
        } else {
          didNotRenderColor = notOnLineColor;
        }

        if (didNotRenderColor != null) {
          this.lcd_.SetPixel(161 + i, ly, (Color) didNotRenderColor);
        }
      }
    }


    private byte GetColorId_(byte colorBit, byte lower, byte upper)
      => (byte) ((((upper >> colorBit) & 1) << 1) |
                 ((lower >> colorBit) & 1));

    //mode 0 - BGP FF47
    //mode 1 - OBP0 FF48
    //mode 2 - OBP1 FF49
    private Color GetColor_(byte palette, int colorId) {
      var colorIndex = (palette >> colorId * 2) & 0x3;

      return colorIndex switch {
          0 => Lcd.WHITE,
          1 => Lcd.LIGHT_GRAY,
          2 => Lcd.DARK_GRAY,
          _ => Lcd.BLACK,
      };
    }
  }
}