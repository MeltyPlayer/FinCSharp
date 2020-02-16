using fin.app;
using fin.app.impl.opentk;
using fin.app.node;

namespace simple {

  public class EntryPoint {
    public static void Main() {
      var app = new OpenTkApp();
      app.Launch(new TestScene(app));
    }

    private class TestScene : BScene {
      public TestScene(BApp app) : base(app) {
      }

      public override void Init(SceneInitEvent evt) {
      }
    }
  }
}