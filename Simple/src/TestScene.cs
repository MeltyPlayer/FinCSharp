using fin.app;
using fin.app.events;
using fin.app.node;
using fin.app.scene;
using fin.app.window;
using fin.graphics.camera;
using fin.graphics.color;
using fin.input;
using fin.input.button;
using fin.settings;
using fin.math.geometry;
using fin.math.number;

namespace simple {
  public sealed class TestScene : BScene {
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
      instantiator.NewChild(viewRoot, new TestComponent(evt.App.Input.Cursor));
    }

    private class TestComponent : IComponent {
      private ICursor cursor_;
      private IDirection circularProgress_ = Direction.FromDegrees(0);

      public TestComponent(ICursor cursor) {
        this.cursor_ = cursor;
      }

      [OnTick]
      private void StartTick_(StartTickEvent _) {
        this.circularProgress_.Radians += .02f;
      }

      [OnTick]
      private void RenderForOrthographicCamera_(
          RenderForOrthographicCameraTickEvent evt) {
        evt.Graphics.Primitives.VertexColor(ColorConstants.GREEN);
        evt.Graphics.Render2d.Rectangle(0,
                                        480 - 20,
                                        640 * (float) (.5 + .5 *
                                                       this.circularProgress_
                                                           .NormalX
                                                      ),
                                        20,
                                        true);

        evt.Graphics.Primitives.VertexColor(ColorConstants.WHITE);
        evt.Graphics.Text.Draw(0,
                               0,
                               24,
                               24,
                               "Hello world.\nLine 2.\nTesting hangydoos.\n  Line 4!");

        var position = this.cursor_.Position;
        if (position != null) {
          float radius = this.cursor_.LeftButton.State switch {
              ButtonState.PRESSED  => 8,
              ButtonState.DOWN     => 15,
              ButtonState.RELEASED => 50,
              ButtonState.UP       => 30,
          };

          evt.Graphics.Primitives.VertexColor(ColorConstants.RED);
          evt.Graphics.Render2d.Circle(position.X,
                                       position.Y,
                                       radius,
                                       20,
                                       true);
        }
      }
    }
  }
}