using fin.app;
using fin.app.events;
using fin.app.impl.opentk;
using fin.app.node;

namespace simple {

  public class EntryPoint {
    public static void Main() {
      var app = new OpenTkApp();
      app.Launch(new TestScene(app));
    }

    private sealed class TestScene : BScene {
      public TestScene(BApp app) : base(app) {
      }

      protected override void Init(SceneInitEvent evt) {
      }

      [OnTick]
      protected void Render(RenderEvent evt) {
      }
    }
  }
}