using fin.app.node;
using fin.app.window;
using fin.graphics.common;
using fin.graphics.common.color;
using fin.log;
using fin.math.geometry;

using OpenTK.Graphics.OpenGL;

namespace fin.app.impl.opentk {

  public sealed partial class AppOpenTk : IApp {

    private sealed partial class WindowManagerOpenTk : IWindowManager {

      private sealed partial class WindowOpenTk : BComponent, IWindow {
        private void BeginViewport_(IBoundingBox<int> box) {
          GL.Enable(EnableCap.ScissorTest);
          this.viewport_.Push(box);
          this.TryToUpdateViewport_();
        }
        private void EndViewport_() {
          if (this.viewport_.Depth > 1) {
            this.viewport_.Pop();
            this.TryToUpdateViewport_();
          } else {
            GL.Disable(EnableCap.ScissorTest);
          }
        }
        private void TryToUpdateViewport_() {
          var current = this.viewport_.DetermineIntersection()!;

          var width = (int)current.Dimensions.Width;
          var height = (int)current.Dimensions.Height;
          var leftX = (int)current.TopLeft.X;
          var bottomY = this.Height - height - (int)current.TopLeft.Y;

          GL.Viewport(leftX, bottomY, width, height);
          GL.Scissor(leftX, bottomY, width, height);
        }

        public IView NewView(IBoundingBox<int> boundingBox) {
          var view = new WindowOpenTkView(this, boundingBox);
          this.views_.Add(view);
          return view;
        }

        private class WindowOpenTkView : IView {
          private readonly WindowOpenTk parent_;
          private readonly IBoundingBox<int> boundingBox_;

          public WindowOpenTkView(WindowOpenTk parent, IBoundingBox<int> boundingBox) {
            this.parent_ = parent;
            this.boundingBox_ = boundingBox;
          }

          public void Render(IGraphics graphics) {
            this.parent_.BeginViewport_(this.boundingBox_);

            var screen = graphics.Screen;
            screen.Clear(ColorConstants.MAGENTA);

            // TODO: Render camera.

            this.parent_.EndViewport_();
          }
        }
      }
    }
  }
}