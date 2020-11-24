using System.Drawing;

using fin.math.random;

namespace fin.graphics.color {
  public interface IColor {
    uint I { get; }

    byte Rb { get; }
    byte Gb { get; }
    byte Bb { get; }
    byte Ab { get; }

    float Rf { get; }
    float Gf { get; }
    float Bf { get; }
    float Af { get; }
  }

  public struct FinColor : IColor {
    public uint I { get; }

    private FinColor(uint i) {
      this.I = i;
    }

    public static FinColor FromRgba(uint i) => new FinColor(i);

    public static FinColor FromRgbaB(byte r, byte g, byte b, byte a) {
      var rr = (uint) ((r & 0xff) << 24);
      var gg = (uint) ((g & 0xff) << 16);
      var bb = (uint) ((b & 0xff) << 8);
      var aa = (uint) (a & 0xff);
      return FinColor.FromRgba(rr + gg + bb + aa);
    }

    public static FinColor FromRgbB(byte r, byte g, byte b)
      => FinColor.FromRgbaB(r, g, b, 255);

    public static FinColor FromRgbaF(float r, float g, float b, float a)
      => FinColor.FromRgbaB((byte) (r * 255),
                         (byte) (g * 255),
                         (byte) (b * 255),
                         (byte) (a * 255));

    public static FinColor FromRgbF(float r, float g, float b)
      => FinColor.FromRgbaF(r, g, b, 1);

    public static FinColor FromVB(byte v) => FinColor.FromRgbB(v, v, v);

    public static FinColor FromCColor(Color color)
      => FinColor.FromRgbaB(color.R, color.G, color.B, color.A);

    // TODO: Optimize this by deleting it, too many calls to new!
    public static implicit operator FinColor(Color color)
      => FinColor.FromCColor(color);

    public static FinColor Random() {
      var (r, g, b) = FinRandom.Bytes3();
      return FinColor.FromRgbB(r, g, b);
    }

    public byte Rb => (byte) (this.I >> 24 & 0xff);
    public byte Gb => (byte) (this.I >> 16 & 0xff);
    public byte Bb => (byte) (this.I >> 8 & 0xff);
    public byte Ab => (byte) (this.I & 0xff);

    public float Rf => this.Rb / 255f;
    public float Gf => this.Gb / 255f;
    public float Bf => this.Bb / 255f;
    public float Af => this.Ab / 255f;
  }
}