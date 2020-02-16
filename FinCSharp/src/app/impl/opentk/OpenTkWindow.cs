using System;
using fin.app.events;
using fin.app.node;
using fin.graphics.common;
using fin.input;
using fin.input.impl.opentk;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace fin.app.impl.opentk {

  public partial class OpenTkApp : BApp {

    // TODO: I'm not happy with this inheritance. Use encapsulation instead.
    private class OpenTkWindow : BChildAppNode, IWindow {
      private readonly IKeyStateDictionary ksd_;

      private readonly INativeWindow window_;
      private readonly IGraphicsContext glContext_;

      public OpenTkWindow(BApp app, int width, int height, IKeyStateDictionary ksd, Action onClose) : base(app) {
        this.window_ = new NativeWindow(width,
          height,
          "SimpleGame",
          GameWindowFlags.Default,
          GraphicsMode.Default,
          DisplayDevice.Default) {
          Visible = true
        };

        this.ksd_ = ksd;
        this.window_.KeyDown += (sender, args) => ksd.OnKeyDown(OpenTkKeyToKeyIdConverter.Convert(args.Key));
        this.window_.KeyUp += (sender, args) => ksd.OnKeyUp(OpenTkKeyToKeyIdConverter.Convert(args.Key));

        this.window_.Closed += (s, e) => onClose();

        var windowInfo = this.window_.WindowInfo;
        this.glContext_ = new GraphicsContext(GraphicsMode.Default,
          windowInfo,
          1,
          0,
          GraphicsContextFlags.Default);
        this.glContext_.MakeCurrent(windowInfo);
        ((IGraphicsContextInternal)this.glContext_).LoadAll();
      }

      // TODO: Come up with a naming convention for OnTick events.
      [OnTick]
      private void StartTick_(StartTickEvent phase) {
        this.window_.ProcessEvents();
        this.ksd_.HandleTransitions();
      }

      [OnTick]
      private void Render_(RenderEvent evt) {
        var g = evt.G;

        this.glContext_.MakeCurrent(this.window_.WindowInfo);

        GL.Enable(EnableCap.DepthTest);

        g.S.Clear(Color.FromRgbF(0, 1, 1));
        GL.Viewport(0, 0, this.Width, this.Height);

        this.glContext_.SwapBuffers();
      }

      public string Title {
        get => this.window_.Title;
        set => this.window_.Title = value;
      }

      public int Width {
        get => this.window_.Width;
        set => this.window_.Width = value;
      }

      public int Height {
        get => this.window_.Height;
        set => this.window_.Height = value;
      }

      public void Close() => this.window_.Close();
    }
  }
}