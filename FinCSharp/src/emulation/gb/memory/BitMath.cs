namespace fin.emulation.gb.memory {
  public static class BitMath {
    public static bool GetBit(int value, int index)
      => ((value >> index) & 0x1) == 0x1;

    public static int SetBit(int value, int index, bool bit)
      => bit
             ? (byte) (value | (0x1 << index))
             : (byte) ~(~value | (0x1 << index));


    public static int CreateBitmaskAt(int bitCount) => 1 << bitCount;

    public static int CreateBitmaskUpTo(int bitCount)
      => BitMath.CreateBitmaskAt(bitCount) - 1;


    /// <summary>
    ///   Shifts the bits right without modifying the sign bit (which is MSB).
    /// </summary>
    public static int ArithmeticShiftRight(
        int value,
        int bitCount,
        out bool carry) {
      carry = (value & 1) == 1;

      var signBit = value & BitMath.CreateBitmaskAt(bitCount - 1);

      var bitMaskUpToEnd = BitMath.CreateBitmaskUpTo(bitCount);
      return signBit & ((value & bitMaskUpToEnd) >> 1);
    }

    /// <summary>
    ///   Shifts all bits left.
    /// </summary>
    public static int LogicalShiftLeft(
        int value,
        int bitCount,
        out bool carry) {
      carry = ((value >> (bitCount - 1)) & 1) == 1;

      var bitMaskUpToEnd = BitMath.CreateBitmaskUpTo(bitCount);
      return (value << 1) & bitMaskUpToEnd;
    }

    /// <summary>
    ///   Shifts all bits right.
    /// </summary>
    public static int LogicalShiftRight(
        int value,
        int bitCount,
        out bool carry) {
      carry = (value & 1) == 1;

      var bitMaskUpToEnd = BitMath.CreateBitmaskUpTo(bitCount);
      return (value & bitMaskUpToEnd) >> 1;
    }


    public static int RotateLeft(int value, int bitCount, out bool carry) {
      var bitMaskUpToEnd = BitMath.CreateBitmaskUpTo(bitCount);

      var remaining = value << 1;
      var carried = value >> (bitCount - 1);

      carry = (carried & 1) == 1;
      return (remaining | carried) & bitMaskUpToEnd;
    }

    public static int RotateRight(int value, int bitCount, out bool carry) {
      var bitMaskUpToEnd = BitMath.CreateBitmaskUpTo(bitCount);
      value &= bitMaskUpToEnd;

      var remaining = value >> 1;
      var carried = value << (bitCount - 1);

      carry = (value & 1) == 1;
      return (remaining | carried) & bitMaskUpToEnd;
    }


    public static int RotateLeftThroughCarry(
        int value,
        int bitCount,
        ref bool carry) {
      var bitMaskUpToEnd = BitMath.CreateBitmaskUpTo(bitCount);

      var remaining = value << 1;
      var overflow = value >> (bitCount - 1);
      var carried = carry ? 1 : 0;

      carry = (overflow & 1) == 1;
      return (remaining | carried) & bitMaskUpToEnd;
    }

    public static int RotateRightThroughCarry(
        int value,
        int bitCount,
        ref bool carry) {
      var bitMaskUpToEnd = BitMath.CreateBitmaskUpTo(bitCount);
      var bitMaskAtMsb = BitMath.CreateBitmaskAt(bitCount - 1);
      value &= bitMaskUpToEnd;

      var remaining = value >> 1;
      var carried = carry ? bitMaskAtMsb : 0;

      carry = (value & 1) == 1;
      return (remaining | carried) & bitMaskUpToEnd;
    }
  }
}