namespace fin.emulation.gb.memory {
  public static class ByteMath {
    public static (byte, byte) SplitShort(ushort value)
      => ((byte) (value >> 8), (byte) (value & 0xff));

    public static ushort MergeBytes(byte upper, byte lower)
      => (ushort) ((upper << 8) | lower);


    public static bool GetBit(byte value, int index)
      => BitMath.GetBit(value, index);

    public static byte SetBit(byte value, int index, bool bit)
      => (byte)BitMath.SetBit(value, index, bit);


    public static byte Swap(byte value) => (byte) ((value << 4) | (value >> 4));


    /// <summary>
    ///   Shifts the bits right without modifying the sign bit (which is MSB).
    /// </summary>
    public static byte ArithmeticShiftRight(byte value, out bool carry)
      => (byte) BitMath.ArithmeticShiftRight(value, 8, out carry);

    /// <summary>
    ///   Shifts all bits left.
    /// </summary>
    public static byte LogicalShiftLeft(byte value, out bool carry)
      => (byte) BitMath.LogicalShiftLeft(value, 8, out carry);

    /// <summary>
    ///   Shifts all bits right.
    /// </summary>
    public static byte LogicalShiftRight(byte value, out bool carry)
      => (byte) BitMath.LogicalShiftRight(value, 8, out carry);


    public static byte RotateLeft(byte value, out bool carry)
      => (byte) BitMath.RotateLeft(value, 8, out carry);

    public static byte RotateRight(byte value, out bool carry)
      => (byte) BitMath.RotateRight(value, 8, out carry);


    public static byte RotateLeftThroughCarry(byte value, ref bool carry)
      => (byte) BitMath.RotateLeftThroughCarry(value, 8, ref carry);

    public static byte RotateRightThroughCarry(byte value, ref bool carry)
      => (byte) BitMath.RotateRightThroughCarry(value, 8, ref carry);
  }
}