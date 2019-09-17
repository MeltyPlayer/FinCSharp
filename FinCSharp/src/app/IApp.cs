using fin.dispose;

namespace fin.app {
  // TODO: Use IDisposableParent
  public abstract class IApp : IFinDisposable {
    public abstract void Launch(IScene room);
  }
}