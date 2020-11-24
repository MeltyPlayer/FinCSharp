using fin.graphics;
using fin.graphics.color;

namespace simple.platformer.player {
  public class BoxPlayerRenderer {
    public PlayerRigidbody PlayerRigidbody { get; set; }
    
    public FinColor Color { get; set; }

    public void Render(IGraphics g) {
      g.Primitives.VertexColor(this.Color);
      g.Render2d.Rectangle((int) this.PlayerRigidbody.LeftX,
                           (int) this.PlayerRigidbody.TopY,
                           (int) this.PlayerRigidbody.Width,
                           (int) this.PlayerRigidbody.Height,
                           false);
      // TODO: Reset vertex color?
    }
  }
}