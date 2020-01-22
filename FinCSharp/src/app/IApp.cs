using fin.discard;

namespace fin.app {

  // TODO: Use IDisposableParent
  public abstract class IApp : Discardable {

    public abstract void Launch(Scene room);
  }
}