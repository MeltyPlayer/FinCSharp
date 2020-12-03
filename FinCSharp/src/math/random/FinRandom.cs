using System;

namespace fin.math.random {
  public static class FinRandom {
    private static readonly Random impl_ = new Random();
    private static readonly byte[] byte_ = new byte[1];
    private static readonly byte[] bytes3_ = new byte[3];

    public static byte Byte() {
      var b = FinRandom.byte_;
      FinRandom.impl_.NextBytes(b);
      return b[0];
    }

    public static (byte, byte, byte) Bytes3() {
      var b3 = FinRandom.bytes3_;
      FinRandom.impl_.NextBytes(b3);
      return (b3[0], b3[1], b3[2]);
    }

    public static int Int() => FinRandom.impl_.Next();

    /// <summary>
    ///   Returns a random floating point number: 0 <= x < 1
    /// </summary>
    public static double Double() => FinRandom.impl_.NextDouble();
  }
}
