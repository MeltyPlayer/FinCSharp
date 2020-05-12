using System;
using System.Collections.Generic;

using fin.app.events;
using fin.app.node;
using fin.app.window;
using fin.input.impl.opentk;
using fin.math.geometry;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Platform;

namespace fin.app.impl.opentk {
  public sealed partial class AppOpenTk {
    private sealed partial class WindowManagerOpenTk {
      // TODO: Separate keystate dictionaries?
      // TODO: App should only be closed when the final window is closed.
      // TODO: What if a window never opens??? Need a safety measure to auto-close.
      public IWindow NewWindow(IWindowArgs args)
        => this.parent_.Instantiator.Wrap(this.parent_.root_,
                                          new WindowOpenTk(
                                              args,
                                              this.parent_.input_,
                                              this.parent_.ScheduleCloseApp_));

      private sealed partial class WindowOpenTk : BComponent, IWindow {
        private readonly INativeWindow nativeWindow_;

        private readonly IGraphicsContext glContext_;

        private readonly MutableBoundingBox<int> windowBoundingBox_;

        private readonly INestedBoundingBoxes<int> viewport_ =
            new NestedBoundingBoxes<int>();

        private readonly IList<WindowOpenTkView> views_ =
            new List<WindowOpenTkView>();

        public WindowOpenTk(IWindowArgs args,
                            InputOpenTk input,
                            Action onClose) {
          var initialWidth = args.Dimensions.Width;
          var initialHeight = args.Dimensions.Height;

          this.nativeWindow_ = new NativeWindow(initialWidth,
                                                initialHeight,
                                                "SimpleGame",
                                                GameWindowFlags.Default,
                                                GraphicsMode.Default,
                                                DisplayDevice.Default);

          this.AttachToNativeInputEvents_(input);

          this.nativeWindow_.Closed += (_, _2) => onClose();

          var windowInfo = this.nativeWindow_.WindowInfo;
          this.glContext_ = new GraphicsContext(GraphicsMode.Default,
                                                windowInfo,
                                                1,
                                                0,
                                                GraphicsContextFlags.Default);
          this.glContext_.MakeCurrent(windowInfo);
          ((IGraphicsContextInternal) this.glContext_).LoadAll();

          //this.glContext_.SwapInterval = 0;

          this.windowBoundingBox_ =
              new MutableBoundingBox<int>(0, 0, initialWidth, initialHeight);
          var windowTopLeft = this.windowBoundingBox_.TopLeft;
          var windowDimensions = this.windowBoundingBox_.Dimensions;
          this.nativeWindow_.Move += (_, _2) =>
              (windowTopLeft.X,
               windowTopLeft.Y) =
              (this.nativeWindow_.X, this.nativeWindow_.Y);
          this.nativeWindow_.Resize += (_, _2) =>
              (windowDimensions.Width, windowDimensions.Height) =
              (this.nativeWindow_.Width, this.nativeWindow_.Height);

          this.viewport_.Push(new AggregationBoundingBox<int>(
                                  new ImmutableVector2<int>(0, 0),
                                  this.windowBoundingBox_.Dimensions));
        }

        private void AttachToNativeInputEvents_(InputOpenTk input) {
          var keyboard = input.Keyboard;
          this.nativeWindow_.KeyDown += (_, args) =>
              keyboard[KeyToKeyIdConverterOpenTk.Convert(args.Key)].Down();
          this.nativeWindow_.KeyUp += (_, args) =>
              keyboard[KeyToKeyIdConverterOpenTk.Convert(args.Key)].Up();

          var cursor = input.Cursor;
          this.nativeWindow_.MouseMove += (_, args) =>
              (cursor.Position.X, cursor.Position.Y) = (args.X, args.Y);
          this.nativeWindow_.MouseEnter += (_, _2) => cursor.Window = this;
          this.nativeWindow_.MouseLeave += (_, _2) => {
            if (cursor.Window == this) {
              cursor.Window = null;
            }
          };
          this.nativeWindow_.MouseDown += (_, args) => {
            if (args.Button == MouseButton.Left) {
              cursor.LeftButton.Down();
            }
            else if (args.Button == MouseButton.Right) {
              cursor.RightButton.Down();
            }
          };
          this.nativeWindow_.MouseUp += (_, args) => {
            if (args.Button == MouseButton.Left) {
              cursor.LeftButton.Up();
            }
            else if (args.Button == MouseButton.Right) {
              cursor.RightButton.Up();
            }
          };
        }

        protected override void Discard() {
          this.nativeWindow_.Close();

          this.nativeWindow_.Dispose();
          this.glContext_.Dispose();
        }

        public void ProcessEvents() {
          if (!this.nativeWindow_.Exists) {
            return;
          }
          this.nativeWindow_.ProcessEvents();
        }

        [OnTick]
        private void TriggerRenderViews_(TriggerRenderViewsTickEvent evt) {
          if (!this.nativeWindow_.Exists) {
            return;
          }
          this.glContext_.MakeCurrent(this.nativeWindow_.WindowInfo);

          GL.Enable(EnableCap.Blend);
          GL.BlendFunc(BlendingFactor.SrcAlpha,
                       BlendingFactor.OneMinusSrcAlpha);

          GL.Enable(EnableCap.AlphaTest);
          GL.Enable(EnableCap.DepthTest);
          GL.DepthFunc(DepthFunction.Lequal);

          GL.Enable(EnableCap.Texture2D);

          foreach (var view in this.views_) {
            view.Render(evt.Graphics);
          }

          this.glContext_.SwapBuffers();
        }

        public string Title {
          get => this.nativeWindow_.Title;
          set => this.nativeWindow_.Title = value;
        }

        public int Width {
          get => (int) this.windowBoundingBox_.Dimensions.Width;
          set => this.windowBoundingBox_.Dimensions.Width =
                     this.nativeWindow_.Width = value;
        }

        public int Height {
          get => (int) this.windowBoundingBox_.Dimensions.Height;
          set => this.windowBoundingBox_.Dimensions.Height =
                     this.nativeWindow_.Height = value;
        }

        public bool Visible {
          get => this.nativeWindow_.Visible;
          set => this.nativeWindow_.Visible = value;
        }

        public void Close() => this.nativeWindow_.Close();
      }
    }
  }
}