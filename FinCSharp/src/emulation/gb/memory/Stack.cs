namespace fin.emulation.gb.memory {
  public class Stack: IStack {
    private readonly IRam ram_;
    private readonly IDoubleRegister sp_;

    public Stack(IRam ram, IDoubleRegister sp) {
      this.ram_ = ram;
      this.sp_ = sp;
    }

    // Grows downward, decrements before push.
    public void Push8(byte value) => this.ram_[--this.sp_.Value] = value;

    public void Push16(ushort value) {
      var (upper, lower) = ByteMath.SplitShort(value);
      this.Push8(lower);
      this.Push8(upper);
    }

    public byte Pop8() => this.ram_[this.sp_.Value++];
    public ushort Pop16() {
      var lower = this.Pop8();
      var upper = this.Pop8();
      return ByteMath.MergeBytes(upper, lower);
    }
  }
}
