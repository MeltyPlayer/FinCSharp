using fin.graphics;
using fin.graphics.color;

namespace simple.platformer.player {
  public class BoxPlayerRenderer {
    public Rigidbody Rigidbody { get; set; }
    public double Size { get; set; }
    public Color Color { get; set; }

    public void Render(IGraphics g) {
      var (x, y) = this.Rigidbody.Position;

      g.Primitives.VertexColor(this.Color);
      g.Render2d.Rectangle((int) (x - this.Size / 2),
                           (int) (480 - (y + this.Size)),
                           (int) this.Size,
                           (int) this.Size,
                           false);
      // TODO: Reset vertex color?
    }
  }
}