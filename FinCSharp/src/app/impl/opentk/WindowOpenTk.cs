using System;
using System.Collections.Generic;

using fin.app.events;
using fin.app.node;
using fin.app.window;
using fin.input;
using fin.input.impl.opentk;
using fin.math.geometry;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace fin.app.impl.opentk {
  public sealed partial class AppOpenTk {
    private sealed partial class WindowManagerOpenTk {
      // TODO: Separate keystate dictionaries?
      // TODO: App should only be closed when the final window is closed.
      // TODO: What if a window never opens??? Need a safety measure to auto-close.
      public IWindow NewWindow(IWindowArgs args)
        => this.parent_.instantiator_.Wrap(this.parent_.root_,
                                           new WindowOpenTk(
                                               args,
                                               this.parent_.ksd_,
                                               this.parent_.CloseApp_));

      private sealed partial class WindowOpenTk : BComponent, IWindow {
        private readonly INativeWindow nativeWindow_;

        private readonly IGraphicsContext glContext_;

        private readonly IKeyStateDictionary ksd_;

        private readonly MutableBoundingBox<int> windowBoundingBox_;

        private readonly INestedBoundingBoxes<int> viewport_ =
            new NestedBoundingBoxes<int>();

        private readonly IList<WindowOpenTkView> views_ =
            new List<WindowOpenTkView>();

        public WindowOpenTk(IWindowArgs args,
                            IKeyStateDictionary ksd,
                            Action onClose) {
          var initialWidth = (int) args.Dimensions.Width;
          var initialHeight = (int) args.Dimensions.Height;

          this.nativeWindow_ = new NativeWindow(initialWidth,
                                                initialHeight,
                                                "SimpleGame",
                                                GameWindowFlags.Default,
                                                GraphicsMode.Default,
                                                DisplayDevice.Default);

          this.ksd_ = ksd;
          this.nativeWindow_.KeyDown += (sender, args) =>
              ksd.OnKeyDown(OpenTkKeyToKeyIdConverter.Convert(args.Key));
          this.nativeWindow_.KeyUp += (sender, args) =>
              ksd.OnKeyUp(OpenTkKeyToKeyIdConverter.Convert(args.Key));

          this.nativeWindow_.Closed += (s, e) => onClose();

          var windowInfo = this.nativeWindow_.WindowInfo;
          this.glContext_ = new GraphicsContext(GraphicsMode.Default,
                                                windowInfo,
                                                1,
                                                0,
                                                GraphicsContextFlags.Default);
          this.glContext_.MakeCurrent(windowInfo);
          ((IGraphicsContextInternal) this.glContext_).LoadAll();

          this.windowBoundingBox_ =
              new MutableBoundingBox<int>(0, 0, initialWidth, initialHeight);
          this.nativeWindow_.Move += (_, _2) => {
            this.windowBoundingBox_.TopLeft.X = this.nativeWindow_.X;
            this.windowBoundingBox_.TopLeft.Y = this.nativeWindow_.Y;
          };
          this.nativeWindow_.Resize += (_, _2) => {
            this.windowBoundingBox_.Dimensions.Width = this.nativeWindow_.Width;
            this.windowBoundingBox_.Dimensions.Height =
                this.nativeWindow_.Height;
          };

          this.viewport_.Push(new AggregationBoundingBox<int>(
                                  new ImmutableVector2<int>(0, 0),
                                  this.windowBoundingBox_.Dimensions));
        }

        protected override void Discard() {
          this.nativeWindow_.Dispose();
          this.glContext_.Dispose();

          // TODO: Should probably close here.
        }

        // TODO: Come up with a naming convention for OnTick events.
        [OnTick]
        private void StartTick_(StartTickEvent _) {
          this.nativeWindow_.ProcessEvents();
          this.ksd_.HandleTransitions();
        }

        [OnTick]
        private void TriggerRenderViews_(TriggerRenderViewsTickEvent evt) {
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