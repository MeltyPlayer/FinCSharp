using fin.app.node;
using fin.app.scene;
using fin.audio;
using fin.graphics;

namespace fin.app {
  using input;

  using window;

  public interface IApp {
    void Launch(IScene room);

    IGraphics Graphics { get; }
    IAudio Audio { get; }
    IInput Input { get; }
    IInstantiator Instantiator { get; }
    IAppWindowManager WindowManager { get; }
  }
}