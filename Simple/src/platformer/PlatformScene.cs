using fin.app.scene;
using fin.app.window;
using fin.settings;
using fin.math.geometry;

using simple.platformer.player;
using simple.platformer.world;

namespace simple.platformer {
  public sealed class PlatformScene : BScene {
    protected override void Init(SceneInitTickEvent evt) {
      var settings = Settings.Load();
      var appWidth = settings.Resolution.Width;
      var appHeight = settings.Resolution.Height;

      var windows =
          evt.App.WindowManager.InitWindowsForScene(
              new WindowArgs().SetDimensions(settings.Resolution));

      var window = windows[0];
      window.Width = appWidth;
      window.Height = appHeight;
      window.Visible = true;

      var instantiator = evt.App.Instantiator;
      var viewRoot = instantiator.NewTopLevelChild();

      var view =
          window.NewView(
              new MutableBoundingBox<int>(0, 0, appWidth, appHeight));
      view.AddOrthographicCamera(viewRoot);

      // Add contents of view.
      var gamepad = evt.App.Input.Controller;
      var playerRigidbody = new PlayerRigidbody {
          Rigidbody = new Rigidbody {
              Position = (LevelConstants.SIZE * 10, LevelConstants.SIZE * 13),
              YAcceleration = PlayerConstants.GRAVITY,
              MaxYSpeed = float.MaxValue,
          },
      };

      var playerStateMachine = new PlayerStateMachine {
          State = PlayerState.STANDING,
      };

      instantiator.NewChild(viewRoot,
                            new PlayerComponent(gamepad,
                                                playerRigidbody,
                                                playerStateMachine),
                            new ItemSwitcherComponent(gamepad,
                                               playerRigidbody,
                                               playerStateMachine));
    }
  }
}