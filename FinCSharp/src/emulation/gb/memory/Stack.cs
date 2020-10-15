namespace fin.emulation.gb.memory {
  public class Stack: IStack {
    private readonly IMemoryMap memoryMap_;
    private readonly IDoubleRegister sp_;

    public Stack(IMemoryMap memoryMap, IDoubleRegister sp) {
      this.memoryMap_ = memoryMap;
      this.sp_ = sp;
    }

    // Grows downward, decrements before push.
    public void Push8(byte value) => this.memoryMap_[--this.sp_.Value] = value;

    public void Push16(ushort value) {
      var (upper, lower) = ByteMath.SplitShort(value);
      this.Push8(lower);
      this.Push8(upper);
    }

    public byte Pop8() => this.memoryMap_[this.sp_.Value++];
    public ushort Pop16() {
      var upper = this.Pop8();
      var lower = this.Pop8();
      return ByteMath.MergeBytes(upper, lower);
    }
  }
}
