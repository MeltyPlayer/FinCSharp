using fin.data.collections.grid;
using fin.graphics;
using fin.graphics.color;

namespace fin.emulation.gb {
  public class Lcd {
    public static readonly Color WHITE = Color.FromVB(255);
    public static readonly Color LIGHT_GRAY = Color.FromVB(190);
    public static readonly Color DARK_GRAY = Color.FromVB(90);
    public static readonly Color BLACK = Color.FromVB(0);

    private readonly FinArrayGrid<Color> pixels_ =
        new FinArrayGrid<Color>(256, 256, Lcd.WHITE);

    public void Reset() => this.pixels_.Clear();

    public Color GetPixel(int x, int y) => this.pixels_[x, y];

    public void SetPixel(int x, int y, Color color)
      => this.pixels_[x, y] = color;

    public void Render(IGraphics g) {
      var r2d = g.Render2d;
      var p = g.Primitives;
      var size = 1;
      p.Begin(PrimitiveType.POINTS);
      for (var y = 0; y < this.pixels_.Height; ++y) {
        for (var x = 0; x < this.pixels_.Width; ++x) {
          var color = this.pixels_[x, y];
          p.VertexColor(color).Vertex(x, y);
          //r2d.Rectangle(x * size, y * size, size, size, true);
        }
      }
      p.End();
    }
  }
}