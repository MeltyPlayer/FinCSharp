using fin.app.events;
using fin.app.node;
using fin.discardable;
using fin.events;

namespace fin.app {

  public class SceneInitEvent : BEvent {
  }

  public abstract class BScene : BComponent {
    [OnTick]
    protected abstract void Init(SceneInitEvent evt);
  }
}