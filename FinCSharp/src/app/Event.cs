using fin.events;
using fin.graphics.common;

namespace fin.app {

  public class StartTickEvent : BEvent {
  }

  public class RenderTickEvent : BEvent {
    public IGraphics Graphics { get; }

    public RenderTickEvent(IGraphics g) {
      this.Graphics = g;
    }
  }
}