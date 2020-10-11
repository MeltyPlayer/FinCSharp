namespace fin.emulation.gb.memory {
  public class Registers {
    public IDoubleRegister Af { get; } = new DoubleRegister {
        Value = ByteMath.MergeBytes(
            // A: 0x01->GB/SGB; 0xFF->GBP; 0x11->GBC
            0x01,
            // F: 0xb0->Z1 N0 H1 C1
            0xb0)
    };

    public ISingleRegister A => this.Af.Upper;
    public ISingleRegister F => this.Af.Lower;

    public bool ZFlag {
      get => this.F.GetBit(7);
      set => this.F.SetBit(7, value);
    }

    public bool NFlag {
      get => this.F.GetBit(6);
      set => this.F.SetBit(6, value);
    }

    public bool HFlag {
      get => this.F.GetBit(5);
      set => this.F.SetBit(5, value);
    }

    public bool CFlag {
      get => this.F.GetBit(4);
      set => this.F.SetBit(4, value);
    }

    public IDoubleRegister Bc { get; } = new DoubleRegister { Value = 0x0013 };
    public ISingleRegister B => this.Bc.Upper;
    public ISingleRegister C => this.Bc.Lower;


    public IDoubleRegister De { get; } = new DoubleRegister {Value = 0x00d8};
    public ISingleRegister D => this.De.Upper;
    public ISingleRegister E => this.De.Lower;


    public IDoubleRegister Hl { get; } = new DoubleRegister {Value = 0x014d};
    public ISingleRegister H => this.Hl.Upper;
    public ISingleRegister L => this.Hl.Lower;

    public IDoubleRegister Sp { get; } = new DoubleRegister {Value = 0xfffe};

    public IDoubleRegister Pc { get; } = new DoubleRegister {Value = 0x0100};
  }
}