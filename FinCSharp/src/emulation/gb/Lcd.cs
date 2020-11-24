using fin.data.collections.grid;
using fin.graphics;
using fin.graphics.color;

namespace fin.emulation.gb {
  public class Lcd {
    public static readonly FinColor WHITE = FinColor.FromVB(255);
    public static readonly FinColor LIGHT_GRAY = FinColor.FromVB(190);
    public static readonly FinColor DARK_GRAY = FinColor.FromVB(90);
    public static readonly FinColor BLACK = FinColor.FromVB(0);

    public bool Active { get; set; } = true;

    public void Reset() {
      this.PixelData.Clear();
    }

    public FinColor GetPixel(int x, int y) => this.PixelData[x, y];

    public void SetPixel(int x, int y, FinColor color)
      => this.PixelData[x, y] = color;

    public IFinGrid<FinColor> PixelData { get; } =
      new FinUnsafeArrayGrid<FinColor>(256, 256, Lcd.WHITE);
  }
}