using fin.app.node;
using fin.app.scene;
using fin.audio;

namespace fin.app {
  using input;

  using window;

  public interface IApp {
    void Launch(IScene room);

    IAudio Audio { get; }
    IInput Input { get; }
    IInstantiator Instantiator { get; }
    IWindowManager WindowManager { get; }
  }
}