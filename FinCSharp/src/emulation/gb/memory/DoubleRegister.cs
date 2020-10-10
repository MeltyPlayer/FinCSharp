namespace fin.emulation.gb.memory {
  public class DoubleRegister : IDoubleRegister {
    public ushort Value {
      get => ByteMath.MergeBytes(this.Upper.Value, this.Lower.Value);
      set => (this.Upper.Value, this.Lower.Value) = ByteMath.SplitShort(value);
    }

    public ISingleRegister Upper { get; } = new SingleRegister();
    public ISingleRegister Lower { get; } = new SingleRegister();
  }
}