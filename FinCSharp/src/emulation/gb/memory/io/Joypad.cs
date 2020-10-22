using fin.emulation.gb.memory.mapper;

namespace fin.emulation.gb.memory.io {
  public sealed class Joypad : IMemoryValue {
    private readonly byte pad_ = 0xf;
    private readonly byte buttons_ = 0xf;

    private byte value_ = 0;

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