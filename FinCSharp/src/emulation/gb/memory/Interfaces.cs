namespace fin.emulation.gb.memory {
  public interface IRam {
    byte this[ushort address] { get; set; }
  }

  public interface IStack {
    void Push8(byte value);
    void Push16(ushort value);

    byte Pop8();
    ushort Pop16();
  }

  public interface ISingleRegister {
    public byte Value { get; set; }

    bool GetBit(int index);
    void SetBit(int index, bool bit);

    byte Swap();

    byte ArithmeticShiftRight(out bool carry);
    byte LogicalShiftLeft(out bool carry);
    byte LogicalShiftRight(out bool carry);

    byte RotateLeft(out bool carry);
    byte RotateRight(out bool carry);

    byte RotateLeftThrough(ref bool carry);
    byte RotateRightThrough(ref bool carry);
  }

  public interface IDoubleRegister {
    public ushort Value { get; set; }

    public ISingleRegister Upper { get; }
    public ISingleRegister Lower { get; }
  }
}