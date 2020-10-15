using System;
using System.IO;

using fin.emulation.gb.memory;
using fin.graphics.color;

namespace fin.emulation.gb {
  public class Cpu {
    private readonly Lcd lcd_;
    private readonly Memory memory_;
    private readonly IOpcodes opcodes_;

    public Cpu(Lcd lcd, Memory memory, IOpcodes opcodes) {
      this.lcd_ = lcd;
      this.memory_ = memory;
      this.opcodes_ = opcodes;

      this.Reset();
    }

    public void Reset() {
      this.ScanlineCycleCounter = 288;  // 456;
    }

    public int ExecuteCycles(int maxCycles)
      => this.Execute_(maxCycles, null);

    public int ExecuteInstructions(int maxInstructions)
      => this.Execute_(null, maxInstructions);

    private int Execute_(int? maxCycles, int? maxInstructions) {
      var totalCycles = 0;
      var totalIterations = 0;

      for (;;) {
        var cyclesThisIteration = 0;

        var memory = this.memory_;
        var ioAddresses = memory.MemoryMap.IoAddresses;
        var ie_ = ioAddresses.Ie;
        var if_ = ioAddresses.If;
        // TODO: Handle interrupts
        var ie_and_if_ = ie_ & if_;
        if (ie_and_if_ != 0) {
          this.memory_.HaltState = HaltState.NOT_HALTED;

          var stack = memory.Stack;
          var pc = memory.Registers.Pc;
          if (memory.InterruptsState == InterruptsState.ON) {
            //v-sync
            if ((ie_and_if_ & 0x1) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              ioAddresses.If = (byte) (ioAddresses.If & ~0x1);

              stack.Push16(pc.Value);
              pc.Value = 0x40;
            }
            //LCD STAT
            else if ((ie_and_if_ & 0x2) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              ioAddresses.If = (byte) (ioAddresses.If & ~0x2);

              stack.Push16(pc.Value);
              pc.Value = 0x48;
            }
            //Timer
            else if ((ie_and_if_ & 0x4) != 4) {
              memory.InterruptsState = InterruptsState.OFF;
              ioAddresses.If = (byte) (ioAddresses.If & ~0x4);

              stack.Push16(pc.Value);
              pc.Value = 0x50;
            }
            //Serial
            else if ((ie_and_if_ & 0x8) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              ioAddresses.If = (byte) (ioAddresses.If & ~0x8);

              stack.Push16(pc.Value);
              pc.Value = 0x58;
            }
            //Joypad
            else if ((ie_and_if_ & 0x10) != 0) {
              memory.InterruptsState = InterruptsState.OFF;
              ioAddresses.If = (byte) (ioAddresses.If & ~0x10);

              stack.Push16(pc.Value);
              pc.Value = 0x60;
            }
          }
        }


        // Enables interrupts if scheduled by EI
        if (this.memory_.InterruptsState ==
            InterruptsState.SCHEDULED_TO_BE_ON) {
          this.memory_.InterruptsState = InterruptsState.ON;
        }


        // Runs instruction if not halted
        if (this.memory_.HaltState != HaltState.HALTED) {
          var cycles = this.opcodes_.FetchAndRunOp();
          cyclesThisIteration += cycles;
        }
        else {
          cyclesThisIteration += 4;
        }


        // Updates timers, LCD w/ usedCycles
        this.updateTimers_(cyclesThisIteration);
        this.updateLcdStatus_(cyclesThisIteration);

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

    private void updateTimers_(int cyclesThisIteration) {
      var ioAddresses = this.memory_.MemoryMap.IoAddresses;

      //DIV
      ioAddresses.Timer1 += cyclesThisIteration;

      if (ioAddresses.Timer1 >= 256) {
        ioAddresses.Timer1 -= 256;
        ++ioAddresses.Div;
      }

      //TIMA
      if ((ioAddresses.Tac & 0x4) != 0) {
        ioAddresses.Timer2 += cyclesThisIteration;

        while (ioAddresses.Timer2 >= this.tacCycles_[ioAddresses.Tac & 0x3]) {
          ioAddresses.Timer2 -= this.tacCycles_[ioAddresses.Tac & 0x3];
          ++ioAddresses.Tima;

          //overflow
          if (ioAddresses.Tima == 0) {
            this.interruptZ80_(InterruptType.TIMER);
            ioAddresses.Tima = ioAddresses.Tma;
          }
        }
      }
    }

    public int ScanlineCycleCounter { get; private set; }

    private void updateLcdStatus_(int cyclesThisIteration) {
      var ioAddresses = this.memory_.MemoryMap.IoAddresses;
      ioAddresses.Stat |= 0x80;

      //lcd enabled
      if ((ioAddresses.Lcdc & 0x80) != 0) {
        this.ScanlineCycleCounter -= cyclesThisIteration;

        //setting modes and requesting interrupts when switching
        if (ioAddresses.Ly >= 144) {
          //set mode to 1
          ioAddresses.Stat = (byte) ((ioAddresses.Stat & 0xFC) | 0x1);
        }
        else {
          int previousmode = ioAddresses.Stat & 0x3;
          int requestinterrupt = 0;

          if (this.ScanlineCycleCounter >= 376) {
            //set mode to 2 (OAM-RAM search)
            ioAddresses.Stat = (byte) ((ioAddresses.Stat & 0xFC) | 0x2);
            requestinterrupt = ioAddresses.Stat & 0x20;
          }
          else if (this.ScanlineCycleCounter >= 204) {
            //set mode to 3 (Data Transfer)
            ioAddresses.Stat = (byte) ((ioAddresses.Stat & 0xFC) | 0x3);
          }
          else {
            //set to mode 0 (H-Blank)
            ioAddresses.Stat &= 0xFC;
            requestinterrupt = ioAddresses.Stat & 0x8;
          }

          //request interrupt when mode changed for the 1st time
          if (requestinterrupt != 0 &&
              (previousmode != (ioAddresses.Stat & 0x3))) {
            this.interruptZ80_(InterruptType.LCD_STAT);
          }
        }

        //move to next scanline
        if (this.ScanlineCycleCounter <= 0) {
          ++ioAddresses.Ly;
          this.ScanlineCycleCounter = 456;

          //starting vsync period
          if (ioAddresses.Ly == 144) {
            //request vblank
            this.interruptZ80_(InterruptType.V_BLANK);

            //set mode to 1
            ioAddresses.Stat = (byte) ((ioAddresses.Stat & 0xFC) | 0x1);
            return;
          }

          if (ioAddresses.Ly < 144) {
            this.DrawScanline_();
          }

          ioAddresses.Ly = (byte) (ioAddresses.Ly % 154);
        }

        this.LyLyc_();
      }
      else {
        //set mode to 0
        ioAddresses.Stat = (byte) (ioAddresses.Stat & 0xF8);
        //reset counter
        this.ScanlineCycleCounter = 456;
        //reset LY
        ioAddresses.Ly = 0;
      }
    }

    private void LyLyc_() {
      var ioAddresses = this.memory_.MemoryMap.IoAddresses;

      //LY=LYC?
      if (ioAddresses.Ly == ioAddresses.Lyc) {
        //STAT
        ioAddresses.Stat |= 0x4;
        if ((ioAddresses.Stat & 0x40) != 0) {
          this.interruptZ80_(InterruptType.LCD_STAT);
        }
      }
      else {
        ioAddresses.Stat = (byte) (ioAddresses.Stat & ~0x4);
      }
    }

    private enum InterruptType {
      V_BLANK,
      LCD_STAT,
      TIMER,
      SERIAL,
      JOYPAD,
    }

    private void interruptZ80_(InterruptType type) {
      var ioAddresses = this.memory_.MemoryMap.IoAddresses;
      switch (type) {
        case InterruptType.V_BLANK:
          ioAddresses.If |= 0x1;
          break;
        case InterruptType.LCD_STAT:
          ioAddresses.If |= 0x2;
          break;
        case InterruptType.TIMER:
          ioAddresses.If |= 0x4;
          break;
        case InterruptType.SERIAL:
          ioAddresses.If |= 0x8;
          break;
        case InterruptType.JOYPAD:
          ioAddresses.If |= 0x10;
          break;
      }
    }

    private void DrawScanline_() {
      var lcdc = this.memory_.MemoryMap.IoAddresses.Lcdc;

      if ((lcdc & 0x1) != 0) {
        this.DrawBg_();
      }

      if ((lcdc & 0x2) != 0) {
        this.DrawSprites_();
      }
    }

    private readonly byte[] wrapper_ = new byte[1];

    private void DrawBg_() {
      int i;
      var useWindow = false;

      var memoryMap = this.memory_.MemoryMap;
      var ioAddresses = memoryMap.IoAddresses;
      var lcdc = ioAddresses.Lcdc;
      var wy = ioAddresses.Wy;
      var ly = ioAddresses.Ly;

      //window enabled and scanline within window ?
      byte yPos;
      int backgroundAddress;
      if ((lcdc & (1 << 5)) != 0 && wy <= ly) {
        yPos = (byte) (ly - wy);

        //Window Tile Map Display Select
        backgroundAddress = ((lcdc & (1 << 6)) != 0)
                                //0x9c00
                                ? 0x1C00
                                //0x9800
                                : 0x1800;

        useWindow = true;
      }
      else //not using window
      {
        yPos = (byte) (ioAddresses.ScY + ly);

        //Window Tile Map Display Select
        backgroundAddress = ((lcdc & (1 << 3)) != 0)
                                //0x9c00
                                ? 0x1C00
                                //0x9800
                                : 0x1800;
      }


      //TODO: testing divide by 8 == multiply by 0.125
      //rowPos o current scanline (of the 8 pixels)
      var rowPos = ((byte) (yPos / 8)) * 32;


      //draw de 160 pixels in current line  TODO: (4 by 4)
      for (i = 0; i < 160; i++) {
        var xPos = i + ioAddresses.ScX;

        if (useWindow) {
          var rWX = ioAddresses.Wx - 7;
          if (i >= rWX) {
            xPos = i - rWX;
          }
        }

        //TODO: testing divide by 8 == multiply by 0.125
        var colPos = (xPos / 8);


        // get the tile identity number
        // which tile data are we using?
        ushort tileAddress;
        if ((lcdc & (1 << 4)) != 0) {
          tileAddress = 0x0;
          var tileNumber =
              memoryMap[
                  (ushort) (0x8000 + backgroundAddress + rowPos + colPos)];
          tileAddress = (ushort) (tileAddress + tileNumber * 16);
        }
        else {
          tileAddress = 0x800;
          
          var uTileNumber =
              memoryMap[
                  (ushort) (0x8000 + backgroundAddress + rowPos + colPos)];
          this.wrapper_[0] = uTileNumber;
          var tileNumber = ((sbyte[]) (Array) this.wrapper_)[0];

          tileAddress = (ushort)(tileAddress + (tileNumber + 128) * 16);
        }

        // each vertical line takes up two bytes of memory
        var line = (yPos % 8) * 2;

        //vram (0x8000 +(tileLocation*16))+line
        var vramAddress = (ushort) (0x8000 + tileAddress + line);
        var data1 = memoryMap[vramAddress];
        var data2 = memoryMap[(ushort) (vramAddress + 1)];

        var colorBit = ((xPos % 8) - 7) * -1;
        // combine data 2 and data 1 to get the colour id for this pixel
        var colorNumber = (data2 & (1 << colorBit)) != 0 ? 0x2 : 0;
        colorNumber |= (data1 & (1 << colorBit)) != 0 ? 1 : 0;


        //finaly get color from palette and draw
        var color = this.GetColor_(colorNumber, 0);

        //draw
        this.lcd_.SetPixel(i, ly, color);
      }
    }

    private void DrawSprites_() {
      var memoryMap = this.memory_.MemoryMap;
      var ioAddresses = memoryMap.IoAddresses;
      int i;

      //loop throught the 40 sprites
      for (i = 0; i < 40; i++) {
        //4 bytes in OAM (Sprite attribute table)
        var index = i * 4;

        // Sprite attrib memory (OAM) address
        var oamAddress = (ushort) (0xfe00 + index);
        var posY = memoryMap[oamAddress] - 16;
        var posX = memoryMap[(ushort) (oamAddress + 1)] - 8;
        var tileLocation = memoryMap[(ushort) (oamAddress + 2)];
        var attributes = memoryMap[(ushort) (oamAddress + 3)];

        //check y - Size in LCDC
        var sizeY = (ushort) (ioAddresses.Lcdc & 0x4) != 0 ? 16 : 8;

        //check if sprite is in current Scanline
        var ly = ioAddresses.Ly;
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
    private Color GetColor_(int number, int mode) {
      // Number will be from 0-3, so these will be from (1, 0) to (7, 6)
      var lo = number * 2;
      var hi = lo + 1;

      var ioAddresses = this.memory_.MemoryMap.IoAddresses;
      var palette = mode switch {
          0 => ioAddresses.Bgp,
          1 => ioAddresses.Obp0,
          2 => ioAddresses.Obp1,
          _ => 0,
      };

      var colorindex = (palette & (1 << hi)) != 0 ? 0x2 : 0;
      colorindex |= (palette & (1 << lo)) != 0 ? 0x1 : 0;

      return colorindex switch {
          0 => Lcd.WHITE,
          1 => Lcd.LIGHT_GRAY,
          2 => Lcd.DARK_GRAY,
          _ => Lcd.BLACK,
      };
    }
  }
}