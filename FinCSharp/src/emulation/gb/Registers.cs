namespace fin.emulation.gb {
  public class Registers {
    public DoubleRegister Bc { get; } = new DoubleRegister();
    public SingleRegister B => this.Bc.Upper;
    public SingleRegister C => this.Bc.Lower;


    public DoubleRegister De { get; } = new DoubleRegister();
    public SingleRegister D => this.De.Upper;
    public SingleRegister E => this.De.Lower;


    public DoubleRegister Hl { get; } = new DoubleRegister();
    public SingleRegister H => this.Hl.Upper;
    public SingleRegister L => this.Hl.Lower;
  }
}