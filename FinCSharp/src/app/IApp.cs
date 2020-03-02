using fin.app.node;
using fin.app.scene;

namespace fin.app {
  using input;

  using window;

  public interface IApp {
    void Launch(IScene room);

    IInput Input { get; }
    IInstantiator Instantiator { get; }
    IWindowManager WindowManager { get; }
  }
}