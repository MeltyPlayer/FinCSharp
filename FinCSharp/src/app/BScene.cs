using fin.app.events;
using fin.app.node;
using fin.events;

namespace fin.app {

  public class SceneInitEvent : BEvent {
  }

  // TODO: I'm not happy with this inheritance. Use encapsulation instead.
  public abstract class BScene : BChildAppNode {
    public BScene(BApp app) : base(app) {
    }

    [OnTick]
    protected abstract void Init(SceneInitEvent evt);
  }
}