using fin.app;
using fin.app.node;
using fin.app.scene;
using fin.app.window;
using fin.settings;
using fin.math.geometry;

using simple.platformer.player;
using simple.platformer.player.sfx;
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
      this.InstantiatePlayer_(evt.App, viewRoot);
    }

    private void InstantiatePlayer_(IApp app, IAppNode viewRoot) {
      var instantiator = app.Instantiator;

      var gamepad = app.Input.Controller;
      var rigidbody = new Rigidbody {
          Position = (LevelConstants.SIZE * 10, LevelConstants.SIZE * 13),
          YAcceleration = PlayerConstants.GRAVITY,
          MaxYSpeed = float.MaxValue,
      };
      var playerRigidbody = new PlayerRigidbody {
          Rigidbody = rigidbody,
      };

      var playerStateMachine = new PlayerStateMachine {
          State = PlayerState.STANDING,
      };

      var playerSounds =
          new PlayerSoundsComponent(app.Audio,
                                    rigidbody,
                                    playerStateMachine);
      var player = new PlayerComponent(gamepad,
                                       playerRigidbody,
                                       playerSounds,
                                       playerStateMachine);
      var itemSwitcher = new ItemSwitcherComponent(gamepad,
                                                   playerRigidbody,
                                                   playerSounds,
                                                   playerStateMachine);
      var vectorRenderer =
          new PlayerVectorRendererComponent(playerRigidbody,
                                            playerStateMachine);

      instantiator.NewChild(viewRoot,
                            player,
                            playerSounds,
                            itemSwitcher,
                            vectorRenderer);
    }
  }
}