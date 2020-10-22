using System;
using System.Collections.Generic;
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
        ++ioAddresses.Div;
      }

      //TIMA
      if ((ioAddresses.Tac & 0x4) != 0) {
        ioAddresses.TimerCounter += cyclesThisIteration;

        var tacCycles = this.tacCycles_[ioAddresses.Tac & 0x3];
        while (ioAddresses.TimerCounter >= tacCycles) {
          ioAddresses.TimerCounter -= tacCycles;

          //overflow
          if (++ioAddresses.Tima == 0) {
            this.InterruptZ80(InterruptType.TIMER);
            ioAddresses.Tima = ioAddresses.Tma;
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
              this.ChangeStat_(PpuModeType.H_BLANK);
              this.PpuModeCycleCount = 456 - this.UpwardScanlineCycleCounter;

              this.DrawScanline_();
            }
          } else if (mode == PpuModeType.H_BLANK) {
            // 80 + 172 + 204
            if (this.UpwardScanlineCycleCounter > 456) {
              this.UpwardScanlineCycleCounter -= 456;

              //starting vsync period
              if (++ly >= 144) {
                this.ChangeStat_(PpuModeType.V_BLANK);
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
          } else if (mode == PpuModeType.V_BLANK) {
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
        stat.Mode = PpuModeType.H_BLANK;
        this.UpwardScanlineCycleCounter = 0;
        ioAddresses.Ly.Value = 0;
      }
    }

    private void ChangeStat_(PpuModeType newModeType) {
      var stat = this.IoAddresses.Stat;
      stat.Mode = newModeType;

      var shouldTriggerLcdInterrupt = newModeType switch {
          PpuModeType.OAM_RAM_SEARCH => stat.OamRamSearchInterruptEnabled,
          PpuModeType.H_BLANK        => stat.HBlankInterruptEnabled,
          PpuModeType.V_BLANK        => stat.VBlankInterruptEnabled,
          _                          => false,
      };

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

      var objCount = 40;
      for (var i = 0; i < objCount; ++i) {
        /* Put the visible sprites into line_obj. Insert them so sprites with
         * smaller X-coordinates are earlier, but only on DMG. On CGB, they are
         * always ordered by obj index. */

        // Sprite attrib memory (OAM) address
        var index = i * 4;
        var oamAddress = (ushort) (0xfe00 + index);
        var objY = memoryMap[oamAddress] - 16;

        var relY = ly - objY;

        if (relY < objHeight) {
          var objX = memoryMap[(ushort) (oamAddress + 1)] - 8;

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

      var scX = ioAddresses.ScX;
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
      var wy = ioAddresses.Wy;
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
        y = (byte) (ioAddresses.ScY + ly);

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

      var scX = ioAddresses.ScX;
      var wX = (byte) (ioAddresses.Wx - 7);

      var areTileAddressesSigned = (lcdc & (1 << 4)) == 0;

      var upper = 0;
      var lower = 0;

      //draw de 160 pixels in current line  TODO: (4 by 4)
      for (var p = 0; p < 160; p++) {
        var x = useWindow && p > wX ? (byte) (p - wX) : (byte) (p + scX);

        if ((p & 7) == 0 || ((p + scX) & 7) == 0) {
          var tileCol = (ushort) (x / 8);
          var tileNumber =
              memoryMap[(ushort) (0x8000 + backgroundAddress + tileRow + tileCol)];

          ushort tileAddress;
          if (!areTileAddressesSigned) {
            tileAddress = (ushort) (0x8000 + tileNumber * 16);
          } else {
            tileAddress =
                (ushort) (0x8800 +
                          (128 + ByteMath.ByteToSByte(tileNumber)) * 16);
          }

          var vramAddress = (ushort) (tileAddress + tileLine);
          lower = memoryMap[vramAddress];
          upper = memoryMap[(ushort) (vramAddress + 1)];
        }

        var colorBit = 7 - (x & 7);
        var colorId = (((upper >> colorBit) & 1) << 1) |
                      ((lower >> colorBit) & 1);

        var color = this.GetColor_(colorId, 0);

        //draw
        this.lcd_.SetPixel(p, ly, color);
      }
    }

    private void DrawSprites_() {
      var memoryMap = this.MemoryMap;
      var ioAddresses = this.IoAddresses;

      var lcdc = ioAddresses.Lcdc.Value;

      //loop throught the 40 sprites
      for (var i = 0; i < 40; i++) {
        //4 bytes in OAM (Sprite attribute table)
        var index = i * 4;

        // Sprite attrib memory (OAM) address
        var oamAddress = (ushort) (0xfe00 + index);
        var posY = memoryMap[oamAddress] - 16;
        var posX = memoryMap[(ushort) (oamAddress + 1)] - 8;
        var tileLocation = memoryMap[(ushort) (oamAddress + 2)];
        var attributes = memoryMap[(ushort) (oamAddress + 3)];

        //check y - Size in LCDC
        var sizeY = (ushort) (lcdc & 0x4) != 0 ? 16 : 8;

        //check if sprite is in current Scanline
        var ly = ioAddresses.Ly.Value;
        if (ly >= posY && ly < posY + sizeY) {
          var line = ly - posY;

          //flip y-axis ?
          if ((attributes & 0x40) != 0) {
            line = (line - sizeY) * -1;
          }

          // each vertical line takes up two bytes of memory
          line *= 2;

          //vram (0x8000 +(tileLocation*16))+line
          var dataAddress = (ushort) (0x8000 + (tileLocation * 16) + line);
          var data1 = memoryMap[dataAddress];
          var data2 = memoryMap[(ushort) (dataAddress + 1)];

          for (var pixel = 7; pixel >= 0; pixel--) {
            var colorBit = pixel;

            int pos = 7 - pixel + posX;
            if (pos > 159 || pos < 0) {
              continue;
            }

            //flip x-axis ?
            if ((attributes & 0x20) != 0) {
              colorBit = (colorBit - 7) * -1;
            }

            // combine data 2 and data 1 to get the colour id for this pixel
            var colorNumber = (data2 & (1 << colorBit)) != 0 ? 0x2 : 0;
            colorNumber |= (data1 & (1 << colorBit)) != 0 ? 1 : 0;

            //get color from palette
            var color =
                this.GetColor_(colorNumber, ((attributes & 0x10) >> 4) + 1);

            //white = transparent for sprites
            if (color.Rb == 255) {
              continue;
            }

            //dont' draw if behind backgound
            if ((attributes & (1 << 7)) == 0 ||
                this.lcd_.GetPixel(pos, ly).Rb == 255) {
              this.lcd_.SetPixel(pos, ly, color);
            }
          }
        }
      }
    }


    //mode 0 - BGP FF47
    //mode 1 - OBP0 FF48
    //mode 2 - OBP1 FF49
    private Color GetColor_(int colorId, int paletteMode) {
      var ioAddresses = this.IoAddresses;
      var palette = paletteMode switch {
          0 => ioAddresses.Bgp,
          1 => ioAddresses.Obp0,
          2 => ioAddresses.Obp1,
          _ => 0,
      };

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