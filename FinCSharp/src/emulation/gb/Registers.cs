namespace fin.emulation.gb {
  public class Registers {
    public SingleRegister A { get; } = new SingleRegister();
    public SingleRegister F { get; } = new SingleRegister();

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

    public DoubleRegister Bc { get; } = new DoubleRegister();
    public SingleRegister B => this.Bc.Upper;
    public SingleRegister C => this.Bc.Lower;


    public DoubleRegister De { get; } = new DoubleRegister();
    public SingleRegister D => this.De.Upper;
    public SingleRegister E => this.De.Lower;


    public DoubleRegister Hl { get; } = new DoubleRegister();
    public SingleRegister H => this.Hl.Upper;
    public SingleRegister L => this.Hl.Lower;

    public DoubleRegister Sp { get; } = new DoubleRegister();

    public DoubleRegister Pc { get; } = new DoubleRegister();

  }
}