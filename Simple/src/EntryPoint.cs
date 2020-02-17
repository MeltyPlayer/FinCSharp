using fin.app;
using fin.app.events;
using fin.app.impl.opentk;
using fin.app.node;

namespace simple {

  public class EntryPoint {
    public static void Main() {
      var app = new OpenTkApp();
      app.Launch(new TestScene());
    }

    private sealed class TestScene : BScene {
      protected override void Discard() {
      }

      protected override void Init(SceneInitEvent evt) {
      }
    }
  }
}