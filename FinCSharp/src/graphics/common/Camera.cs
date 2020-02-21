using fin.app.node;
using fin.events;

namespace fin.graphics.common {

  public class CameraRenderEvent : BEvent {
    public IGraphics Graphics { get; }
    public ICamera Camera { get; }

    public CameraRenderEvent(IGraphics graphics, ICamera camera) {
      this.Graphics = graphics;
      this.Camera = camera;
    }
  }

  public interface ICamera {
    // TODO: Rethink this.
    void RenderFromEntrypoint(IGraphics graphics, IAppNode entrypoint);
  }

  public interface IOrthographicCamera : ICamera {
    public float Left { get; }
    public float Right { get; }
    public float Top { get; }
    public float Bottom { get; }
    public float Near { get; }
    public float Far { get; }
  }
}