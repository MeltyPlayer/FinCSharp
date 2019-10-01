using System;
using fin.app.phase;
using fin.graphics.common;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace fin.app.impl.opentk {

  public partial class OpenTkApp : IApp {

    private class OpenTkWindow : ReflectivePhaseHandler, IWindow, IRenderHandler {
      private readonly INativeWindow window_;
      private readonly IGraphicsContext glContext_;

      // TODO: Prevent instantiation by anyone other than game.
      public OpenTkWindow(int width, int height, Action onClose) {
        this.window_ = new NativeWindow(width,
          height,
          "SimpleGame",
          GameWindowFlags.Default,
          GraphicsMode.Default,
          DisplayDevice.Default) {
          Visible = true
        };
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

      public void OnPhase(IGraphics g) {
        this.window_.ProcessEvents();

        this.glContext_.MakeCurrent(this.window_.WindowInfo);

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