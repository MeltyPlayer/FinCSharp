using fin.app.node;
using fin.events;

namespace fin.graphics.camera {
  public class RenderForOrthographicCameraTickEvent : BEvent {
    public IGraphics Graphics { get; }
    public IOrthographicCamera Camera { get; }

    public RenderForOrthographicCameraTickEvent(IGraphics graphics,
                                             IOrthographicCamera camera) {
      this.Graphics = graphics;
      this.Camera = camera;
    }
  }


  public interface ICamera {
    void Render(IGraphics graphics);
  }

  public interface IOrthographicCamera : ICamera {
    public float Left { get; set; }
    public float Right { get; set; }
    public float Top { get; set; }
    public float Bottom { get; set; }
    public float Near { get; set; }
    public float Far { get; set; }
  }

  public interface IPerspectiveCamera : ICamera {
  }
}