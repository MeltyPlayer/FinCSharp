using fin.app.impl.opentk;
using fin.app.scene;
using fin.app.window;
using fin.math.geometry;
using fin.settings;

namespace simple {

  public class EntryPoint {
    public static void Main() {
      var app = new AppOpenTk();
      app.Launch(new TestScene());
    }

    private sealed class TestScene : BScene {
      protected override void Discard() {
      }

      protected override void Init(SceneInitTickEvent evt) {
        var settings = Settings.Load();

        var windows = evt.WindowManager.InitWindowsForScene(new WindowArgs().SetDimensions(settings.Resolution));

        var window = windows[0];
        window.Width = settings.Resolution.Width;
        window.Height = settings.Resolution.Height;
        window.Visible = true;

        var view = window.NewView(new MutableBoundingBox<int>(100, 100, 300, 300));
      }
    }
  }
}