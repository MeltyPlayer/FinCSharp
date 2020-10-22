using System.Text;

namespace fin.emulation.gb.memory {
  public static class ByteFormatter {
    public static string ToHex8(byte value) => new StringBuilder(4)
                                             .AppendFormat(
                                                 "0x{0:x2}",
                                                 value)
                                             .ToString();

    public static string ToHex16(ushort value) => new StringBuilder(6)
                                                  .AppendFormat(
                                                      "0x{0:x4}",
                                                      value)
                                                  .ToString();
  }
}