using fin.graphics;
using fin.graphics.color;

namespace simple.platformer.player {
  public class BoxPlayerRenderer {
    public Rigidbody Rigidbody { get; set; }
    public double HSize { get; set; }
    public double VSize { get; set; }
    public Color Color { get; set; }

    public void Render(IGraphics g) {
      var (x, y) = this.Rigidbody.Position;

      g.Primitives.VertexColor(this.Color);
      g.Render2d.Rectangle((int) (x - this.HSize / 2),
                           (int) (y - this.VSize),
                           (int) this.HSize,
                           (int) this.VSize,
                           false);
      // TODO: Reset vertex color?
    }
  }
}