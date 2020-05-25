using CColor = System.Drawing.Color;

namespace fin.graphics.color {
  public static class ColorConstants {
    public static readonly Color RED = CColor.Red;
    public static readonly Color GREEN = CColor.Green;
    public static readonly Color BLUE = CColor.Blue;

    public static readonly Color YELLOW = CColor.Yellow;
    public static readonly Color CYAN = Color.FromRgbF(0, 1, 1);
    public static readonly Color MAGENTA = Color.FromRgbF(1, 0, 1);

    public static readonly Color ORANGE = Color.FromRgbB(255, 165, 0);
    public static readonly Color PURPLE = Color.FromRgbB(165, 0,255);
    public static readonly Color TEAL = Color.FromRgbB(0, 255, 165);

    public static readonly Color WHITE = Color.FromCColor(CColor.White);
    public static readonly Color BLACK = Color.FromRgbF(0, 0, 0);

    public static readonly Color TRANSPARENT_BLACK = Color.FromRgbaF(0, 0, 0, 0);
  }
}