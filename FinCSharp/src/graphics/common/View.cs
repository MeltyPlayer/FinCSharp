using fin.math.geometry;

namespace fin.graphics.common {

  public interface IView {
    void Render(IGraphics graphics);
  }

  /*public class OrthographicView : IView {
    private MutableBoundingBox BoundingBox { get; }
    ICamera

    public void Render(IGraphics graphics) {
      this.graphics.
    }
  }*/
}