using fin.events;
using fin.graphics;

namespace fin.app {
  public class StartTickEvent : BEvent { }

  public class ProcessInputsEvent : BEvent { }
  public class TickPhysicsEvent : BEvent { }
  public class TickCollisionsEvent : BEvent { }
  public class TickAnimationEvent : BEvent { }

  public class TriggerRenderViewsTickEvent : BEvent {
    public IGraphics Graphics { get; }

    public TriggerRenderViewsTickEvent(IGraphics g) {
      this.Graphics = g;
    }
  }
}