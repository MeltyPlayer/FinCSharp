using fin.emulation.gb.memory.mapper;
using fin.input.button;

namespace fin.emulation.gb.memory.io {
  public enum JoypadMask {
    DPAD_RIGHT = 0x11,
    DPAD_LEFT = 0x12,
    DPAD_UP = 0x14,
    DPAD_DOWN = 0x18,

    BUTTON_A = 0x21,
    BUTTON_B = 0x22,

    BUTTON_SELECT = 0x24,
    BUTTON_START = 0x28,
  }

  public sealed class Joypad : IMemoryValue {
    private byte pad_ = 0xf;
    private byte buttons_ = 0xf;

    private byte value_ = 0;

    public void ToggleDpadRight(bool isDown)
      => this.TogglePad_(JoypadMask.DPAD_RIGHT, isDown);

    public void ToggleDpadLeft(bool isDown)
      => this.TogglePad_(JoypadMask.DPAD_LEFT, isDown);

    public void ToggleDpadUp(bool isDown)
      => this.TogglePad_(JoypadMask.DPAD_UP, isDown);

    public void ToggleDpadDown(bool isDown)
      => this.TogglePad_(JoypadMask.DPAD_DOWN, isDown);

    private void TogglePad_(JoypadMask mask, bool isDown) {
      var bitMask = (byte) mask & 0xf;
      if (isDown) {
        this.pad_ = (byte) (this.pad_ & ~bitMask);
      } else {
        this.pad_ = (byte) (this.pad_ | bitMask);
      }
    }

    public void ToggleButtonA(bool isDown)
      => this.ToggleButton_(JoypadMask.BUTTON_A, isDown);

    public void ToggleButtonB(bool isDown)
      => this.ToggleButton_(JoypadMask.BUTTON_B, isDown);

    public void ToggleButtonSelect(bool isDown)
      => this.ToggleButton_(JoypadMask.BUTTON_SELECT, isDown);

    public void ToggleButtonStart(bool isDown)
      => this.ToggleButton_(JoypadMask.BUTTON_START, isDown);

    private void ToggleButton_(JoypadMask mask, bool isDown) {
      var bitMask = (byte) mask & 0xf;
      if (isDown) {
        this.buttons_ = (byte) (this.buttons_ & ~bitMask);
      } else {
        this.buttons_ = (byte) (this.buttons_ | bitMask);
      }
    }


    public void Update(Cpu cpu) {
      if (!BitMath.GetBit(this.value_, 4)) {
        //this.value_ = (byte) ((this.value_ & 0xF0) | this.pad_);
        if (this.pad_ != 0xf) {
          cpu.InterruptZ80(Cpu.InterruptType.JOYPAD);
        }
      }
      if (!BitMath.GetBit(this.value_, 5)) {
        //this.value_ = (byte) ((this.value_ & 0xF0) | this.buttons_);
        if (this.buttons_ != 0xF) {
          cpu.InterruptZ80(Cpu.InterruptType.JOYPAD);
        }
      }
      /*if ((this.value_ & 0b00110000) == 0b00110000) {
        this.value_ = 0xff;
      }*/
    }

    public byte this[ushort address] {
      get => this.Value;
      set => this.Value = value;
    }

    public int Size => 1;

    public byte Value {
      get => this.value_;
      set {
        var mode = (byte) (value & 0x30);
        this.value_ = (byte) (mode |
                              (this.value_ & 0x30) switch {
                                  0x10 => this.pad_,
                                  0x20 => this.buttons_,
                                  _    => 0,
                              });
      }
    }
  }
}