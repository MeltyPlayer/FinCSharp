namespace simple {
  using System.Drawing;

  using fin.app.events;
  using fin.app.node;
  using fin.app.scene;
  using fin.app.window;
  using fin.graphics.camera;
  using fin.graphics.color;
  using fin.input;
  using fin.settings;
  using fin.math.geometry;

  public sealed class TestScene : BScene {
    protected override void Discard() {}

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

    private class TestComponent : BComponent {
      private ICursor cursor_;

      public TestComponent(ICursor cursor) {
        this.cursor_ = cursor;
      }

      protected override void Discard() {}

      [OnTick]
      private void RenderForOrthographicCamera_(
          RenderForOrthographicCameraTickEvent evt) {
        evt.Graphics.Primitives.VertexColor(ColorConstants.GREEN);
        evt.Graphics.Render2d.Rectangle(0, 0, 30, 30, true);

        evt.Graphics.Primitives.VertexColor(ColorConstants.WHITE);
        evt.Graphics.Text.Draw(0,
                               0,
                               24,
                               24,
                               "Hello world.\nLine 2.\nTesting hangydoos.\n  Line 4!");

        var position = this.cursor_.Position;
        if (position != null) {
          evt.Graphics.Primitives.VertexColor(ColorConstants.RED);
          evt.Graphics.Render2d.Circle(position.X,
                                       position.Y,
                                       10,
                                       20,
                                       true);
        }
      }
    }
  }
}