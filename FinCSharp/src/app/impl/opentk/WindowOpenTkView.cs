using fin.graphics;
using fin.graphics.camera;
using fin.math.geometry;

using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

namespace fin.app.impl.opentk {
  using graphics.impl.opentk;

  using node;

  public sealed partial class AppOpenTk {
    private sealed partial class WindowManagerOpenTk {
      private sealed partial class WindowOpenTk {
        private void BeginViewport_(IBoundingBox<int> box) {
          GL.Enable(EnableCap.ScissorTest);
          this.viewport_.Push(box);
          this.TryToUpdateViewport_();
        }

        private void EndViewport_() {
          if (this.viewport_.Depth > 1) {
            this.viewport_.Pop();
            this.TryToUpdateViewport_();
          }
          else {
            GL.Disable(EnableCap.ScissorTest);
          }
        }

        private void TryToUpdateViewport_() {
          var current = this.viewport_.DetermineIntersection()!;

          var width = current.Dimensions.Width;
          var height = current.Dimensions.Height;
          var leftX = current.TopLeft.X;
          var bottomY = this.Height - height - current.TopLeft.Y;

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
          private readonly IList<ICamera> cameras_ = new List<ICamera>();

          public WindowOpenTkView(WindowOpenTk parent,
                                  IBoundingBox<int> boundingBox) {
            this.parent_ = parent;
            this.boundingBox_ = boundingBox;
          }

          public IOrthographicCamera
            AddOrthographicCamera(IAppNode entryPoint) {
            var camera = new OrthographicCameraOpenTk(entryPoint) {
              Left = 0,
              Right = this.boundingBox_.Dimensions.Width,
              Bottom = this.boundingBox_.Dimensions.Height,
              Top = 0,
              Near = -1,
              Far = 1000
            };
            this.cameras_.Add(camera);
            return camera;
          }

          public void Render(IGraphics graphics) {
            this.parent_.BeginViewport_(this.boundingBox_);

            // TODO: Make the clear color configurable.
            graphics.Screen.Clear();

            // TODO: Render camera.
            foreach (var camera in this.cameras_) {
              camera.Render(graphics);
            }

            this.parent_.EndViewport_();
          }
        }
      }
    }
  }
}