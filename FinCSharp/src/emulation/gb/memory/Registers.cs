namespace fin.emulation.gb.memory {
  public class Registers {
    public ISingleRegister A { get; } = new SingleRegister();
    public ISingleRegister F { get; } = new SingleRegister();

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

    public IDoubleRegister Bc { get; } = new DoubleRegister();
    public ISingleRegister B => this.Bc.Upper;
    public ISingleRegister C => this.Bc.Lower;


    public IDoubleRegister De { get; } = new DoubleRegister();
    public ISingleRegister D => this.De.Upper;
    public ISingleRegister E => this.De.Lower;


    public IDoubleRegister Hl { get; } = new DoubleRegister();
    public ISingleRegister H => this.Hl.Upper;
    public ISingleRegister L => this.Hl.Lower;

    public IDoubleRegister Sp { get; } = new DoubleRegister {Value = 0xfffe};

    public IDoubleRegister Pc { get; } = new DoubleRegister();
  }
}