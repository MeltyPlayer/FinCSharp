using fin.app.events;
using fin.app.node;

namespace fin.app.scene {
  public interface IScene : IComponent { }

  public abstract class BScene : IComponent, IScene {
    [OnTick]
    protected abstract void Init(SceneInitTickEvent evt);
  }
}