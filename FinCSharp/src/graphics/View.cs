using fin.graphics.camera;

namespace fin.graphics {
  using app.node;

  // TODO: This should probably be discardable.
  public interface IView {
    IOrthographicCamera AddOrthographicCamera(IAppNode entryPoint);

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