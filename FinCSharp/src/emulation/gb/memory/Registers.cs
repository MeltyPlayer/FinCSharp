namespace fin.emulation.gb.memory {
  public class Registers : IRegisters {
    public Registers() {
      this.Reset();
    }

    public void Reset() {
      this.Af.Value = 0x1180; //0x11b0;
      this.Bc.Value = 0x0000; //0x0013;
      this.De.Value = 0xff56; //0x00d8;
      this.Hl.Value = 0x000d; //0x014d;

      this.Sp.Value = 0xfffe;
      this.Pc.Value = 0x0100;
    }


    public IDoubleRegister Af { get; } = new DoubleRegister();

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

    public IDoubleRegister Bc { get; } = new DoubleRegister();
    public ISingleRegister B => this.Bc.Upper;
    public ISingleRegister C => this.Bc.Lower;


    public IDoubleRegister De { get; } = new DoubleRegister();
    public ISingleRegister D => this.De.Upper;
    public ISingleRegister E => this.De.Lower;


    public IDoubleRegister Hl { get; } = new DoubleRegister();
    public ISingleRegister H => this.Hl.Upper;
    public ISingleRegister L => this.Hl.Lower;

    public IDoubleRegister Sp { get; } = new DoubleRegister();

    public IDoubleRegister Pc { get; } = new DoubleRegister();
  }
}