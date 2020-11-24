using CColor = System.Drawing.Color;

namespace fin.graphics.color {
  public static class ColorConstants {
    public static readonly FinColor RED = CColor.Red;
    public static readonly FinColor GREEN = CColor.Green;
    public static readonly FinColor BLUE = CColor.Blue;

    public static readonly FinColor YELLOW = CColor.Yellow;
    public static readonly FinColor CYAN = FinColor.FromRgbF(0, 1, 1);
    public static readonly FinColor MAGENTA = FinColor.FromRgbF(1, 0, 1);

    public static readonly FinColor ORANGE = FinColor.FromRgbB(255, 165, 0);
    public static readonly FinColor PURPLE = FinColor.FromRgbB(165, 0,255);
    public static readonly FinColor TEAL = FinColor.FromRgbB(0, 255, 165);

    public static readonly FinColor WHITE = FinColor.FromCColor(CColor.White);
    public static readonly FinColor BLACK = FinColor.FromRgbF(0, 0, 0);

    public static readonly FinColor TRANSPARENT_BLACK = FinColor.FromRgbaF(0, 0, 0, 0);
  }
}