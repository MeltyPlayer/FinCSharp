using fin.emulation.gb.memory.mapper;

namespace fin.emulation.gb.memory.io {
  public class IoAddresses : IMemorySource {
    private readonly IMemoryMapper impl_;

    public ISerialBus SerialBus { get; }

    public Joypad Joypad { get; } = new Joypad();
    public If If { get; } = new If();
    public Lcdc Lcdc { get; } = new Lcdc();
    public Stat Stat { get; } = new Stat();
    public Ly Ly { get; } = new Ly();
    public Lyc Lyc { get; } = new Lyc();
    public IMemoryValue Ie { get; }

    public IoAddresses(ISerialBus serialBus) {
      this.SerialBus = serialBus;

      /*case 0x44:
        this.data_[relativeAddress] = 0;
        return;*/

      var f = IMemorySourceFactory.INSTANCE;

      this.Ie = f.NewValue();

      this.impl_ = f.BuildMapper(0x100)
                    .Register(0x00, this.Joypad)
                    .Register(
                        0x01,
                        f.BuildValue()
                         .OnSet(this.SerialBus.Bytes.OnNext)
                         .Build()
                    )
                    //FF04 - DIV - Divider Register (R/W)
                    .Register(0x04,
                              f.BuildValue()
                               .OnSet((value, direct) => {
                                 if (!direct) {
                                   this.DivCounter = 0;
                                   return 0;
                                 }
                                 return value;
                               })
                               .Build())
                    //FF07 - TAC
                    .Register(0x07,
                              f.BuildValue()
                               .OnSet((value , direct) => {
                                 if (!direct) {
                                   this.TimerCounter = 0;
                                   return (byte)(value | 0xf8);
                                 }
                                 return value;
                               })
                               .Build())
                    .Register(0x0f, this.If)
                    .Register(0x40, this.Lcdc)
                    .Register(0x41, this.Stat)
                    .Register(0x44, this.Ly)
                    .Register(0x45, this.Lyc)
                    .Register(0xff, this.Ie)
                    .Build();
    }

    public void Reset() {
      this.DivCounter = 0;
      this.TimerCounter = 0;

      for (var i = 0; i < this.impl_.Size; ++i) {
        this.impl_[(ushort) i] = 0;
      }

      this.Div = 0x00;
      this.Tima = 0x00;
      this.Tma = 0x00;
      this.Tac = 0x00;
      this.Nr10 = 0x80;
      this.Nr11 = 0xbf;
      this.Nr12 = 0xf3;
      this.Nr14 = 0xbf;
      this.Nr21 = 0x3f;
      this.Nr22 = 0x00;
      this.Nr24 = 0xbf;
      this.Nr30A = 0x7f;
      this.Nr31 = 0xff;
      this.Nr32 = 0x9f;
      this.Nr33 = 0xbf;
      this.Nr41 = 0xff;
      this.Nr42 = 0x00;
      this.Nr43 = 0x00;
      this.Nr30B = 0xbf;
      this.Nr50 = 0x77;
      this.Nr51 = 0xf3;
      this.Nr52 = 0xf1;
      this.Lcdc.Value = 0x91;
      this.Stat.Value = 0x84;
      this.ScY = 0x00;
      this.ScX = 0x00;

      //this.Ly = 0x90; // 0x00 for DMG, 0x90 for GBC
      this.Ly.Value = 0x00; // 0x00 for DMG, 0x90 for GBC

      this.Lyc.Value = 0x00;
      this.Bgp = 0xfc;
      this.Obp0 = 0xff;
      this.Obp1 = 0xff;
      this.Wy = 0x00;
      this.Wx = 0x00;
      this.Ie.Value = 0x00;
      this.If.Value = 0xe0;
    }

    public byte Div {
      get => this[0x04];
      set => this[0x04] = value;
    }

    public byte Tima {
      get => this[0x05];
      set => this[0x05] = value;
    }

    public byte Tma {
      get => this[0x06];
      set => this[0x06] = value;
    }

    public byte Tac {
      get => this[0x07];
      set => this[0x07] = value;
    }

    public byte Nr10 {
      get => this[0x10];
      set => this[0x10] = value;
    }

    public byte Nr11 {
      get => this[0x11];
      set => this[0x11] = value;
    }

    public byte Nr12 {
      get => this[0x12];
      set => this[0x12] = value;
    }

    public byte Nr14 {
      get => this[0x14];
      set => this[0x14] = value;
    }

    public byte Nr21 {
      get => this[0x16];
      set => this[0x16] = value;
    }

    public byte Nr22 {
      get => this[0x17];
      set => this[0x17] = value;
    }

    public byte Nr24 {
      get => this[0x19];
      set => this[0x19] = value;
    }

    public byte Nr30A {
      get => this[0x1a];
      set => this[0x1a] = value;
    }

    public byte Nr31 {
      get => this[0x1b];
      set => this[0x1b] = value;
    }

    public byte Nr32 {
      get => this[0x1c];
      set => this[0x1c] = value;
    }

    public byte Nr33 {
      get => this[0x1e];
      set => this[0x1e] = value;
    }

    public byte Nr41 {
      get => this[0x20];
      set => this[0x20] = value;
    }

    public byte Nr42 {
      get => this[0x21];
      set => this[0x21] = value;
    }

    public byte Nr43 {
      get => this[0x22];
      set => this[0x22] = value;
    }

    public byte Nr30B {
      get => this[0x23];
      set => this[0x23] = value;
    }

    public byte Nr50 {
      get => this[0x24];
      set => this[0x24] = value;
    }

    public byte Nr51 {
      get => this[0x25];
      set => this[0x25] = value;
    }

    public byte Nr52 {
      get => this[0x26];
      set => this[0x26] = value;
    }

    public byte ScY {
      get => this[0x42];
      set => this[0x42] = value;
    }

    public byte ScX {
      get => this[0x43];
      set => this[0x43] = value;
    }

    public byte Bgp {
      get => this[0x47];
      set => this[0x47] = value;
    }

    public byte Obp0 {
      get => this[0x48];
      set => this[0x48] = value;
    }

    public byte Obp1 {
      get => this[0x49];
      set => this[0x49] = value;
    }

    public byte Wy {
      get => this[0x4a];
      set => this[0x4a] = value;
    }

    public byte Wx {
      get => this[0x4b];
      set => this[0x4b] = value;
    }
    

    public byte this[ushort address] {
      get => this.impl_[address];
      set => this.impl_[address] = value;
    }

    public int Size => this.impl_.Size;

    public int DivCounter { get; set; }

    public int TimerCounter { get; set; }
  }
}