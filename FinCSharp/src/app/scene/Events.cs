using fin.app.window;
using fin.events;

namespace fin.app.scene {

  public class SceneInitTickEvent : BEvent {
    public IWindowManager WindowManager { get; }

    public SceneInitTickEvent(IWindowManager windowManager) {
      this.WindowManager = windowManager;
    }
  }

  public class SceneEndTickEvent : BEvent {
  }
}