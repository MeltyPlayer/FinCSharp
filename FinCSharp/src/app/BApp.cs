using fin.app.node;

namespace fin.app {

  // TODO: I'm not happy with this inheritance. Find a way to encapsulate this instead.
  public abstract class BApp : BRootAppNode {
    public abstract void Launch(BScene room);
  }
}