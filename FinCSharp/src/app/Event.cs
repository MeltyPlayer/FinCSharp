using fin.events;
using fin.graphics.common;

namespace fin.app {

  public class StartTickEvent : BEvent {
  }

  public class RenderEvent : BEvent {
    public IGraphics G { get; }

    public RenderEvent(IGraphics g) {
      this.G = g;
    }
  }
}