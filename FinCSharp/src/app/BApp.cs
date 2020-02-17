using fin.app.node;

namespace fin.app {

  // TODO: I'm not happy with this inheritance. Find a way to encapsulate this instead.
  public interface IApp {
    void Launch(BScene room);
  }
}