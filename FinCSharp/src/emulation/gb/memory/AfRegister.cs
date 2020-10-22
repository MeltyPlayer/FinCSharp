namespace fin.emulation.gb.memory {
  public class AfRegister : IDoubleRegister {
    private readonly IDoubleRegister impl_ = new DoubleRegister();

    public ushort Value {
      get => (ushort) (this.impl_.Value & 0xfff0);
      set => this.impl_.Value = (ushort) (value & 0xfff0);
    }

    public ISingleRegister Upper => this.impl_.Upper;
    public ISingleRegister Lower => this.impl_.Lower;
  }
}