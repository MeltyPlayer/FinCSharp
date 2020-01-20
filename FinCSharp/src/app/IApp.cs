using fin.discard;

namespace fin.app {

  // TODO: Use IDisposableParent
  public abstract class IApp : IDiscardable {

    public abstract void Launch(Scene room);
  }
}