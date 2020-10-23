using fin.data.collections.grid;
using fin.graphics;
using fin.graphics.color;

namespace fin.emulation.gb {
  public class Lcd {
    public static readonly Color WHITE = Color.FromVB(255);
    public static readonly Color LIGHT_GRAY = Color.FromVB(190);
    public static readonly Color DARK_GRAY = Color.FromVB(90);
    public static readonly Color BLACK = Color.FromVB(0);

    public bool Active { get; set; } = true;

    public void Reset() {
      this.PixelData.Clear();
    }

    public Color GetPixel(int x, int y) => this.PixelData[x, y];

    public void SetPixel(int x, int y, Color color)
      => this.PixelData[x, y] = color;

    public IFinGrid<Color> PixelData { get; } =
      new FinUnsafeArrayGrid<Color>(256, 256, Lcd.WHITE);
  }
}