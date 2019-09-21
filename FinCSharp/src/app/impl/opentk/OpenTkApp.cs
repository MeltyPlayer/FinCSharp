using fin.function;
using fin.graphics.common;
using fin.graphics.common.impl.opentk;
using fin.settings;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace fin.app.impl.opentk {
  public class OpenTkApp : IApp {
    private readonly Window window_;
    private readonly RecurrentCaller ticker_;

    private readonly IGraphics g_ = new OpenTkGraphics();

    public OpenTkApp() {
      var settings = Settings.Load();
      this.window_ = new Window(settings.Resolution.Width,
        settings.Resolution.Height,
        this.Dispose);
      this.ticker_ = RecurrentCaller.FromFrequency(settings.Framerate,
        this.Tick_);
      this.OnDisposeEvent += this.ticker_.Stop;
    }

    public override void Launch(IScene scene) {
      GL.Enable(EnableCap.DepthTest);
      GL.ClearColor(Color4.Cyan);

      this.ticker_.Start();
    }

    private void Tick_() {
      this.window_.Title = "SimpleGame (" +
                           (int) this.ticker_.ActualFrequency +
                           ")";

      this.window_.Render(this.g_);
    }

    private class Window : IWindow {
      private readonly INativeWindow window_;
      private readonly IGraphicsContext glContext_;

      // TODO: Prevent instantiation by anyone other than game.
      public Window(int width, int height, Action onClose) {
        this.window_ = new NativeWindow(width,
          height,
          "SimpleGame",
          GameWindowFlags.Default,
          GraphicsMode.Default,
          DisplayDevice.Default);
        this.window_.Visible = true;
        this.window_.Closed += (s, e) => onClose();

        var windowInfo = this.window_.WindowInfo;
        this.glContext_ = new GraphicsContext(GraphicsMode.Default,
          windowInfo,
          1,
          0,
          GraphicsContextFlags.Default);
        this.glContext_.MakeCurrent(windowInfo);
        ((IGraphicsContextInternal) this.glContext_).LoadAll();
      }

      public void Render(IGraphics g) {
        this.glContext_.MakeCurrent(this.window_.WindowInfo);

        g.S.Clear(Color.FromRgbF(0, 1, 1));
        GL.Viewport(0, 0, this.Width, this.Height);


        this.glContext_.SwapBuffers();
      }

      public string Title {
        get { return this.window_.Title; }
        set { this.window_.Title = value; }
      }

      public int Width {
        get { return this.window_.Width; }
        set { this.window_.Width = value; }
      }

      public int Height {
        get { return this.window_.Height; }
        set { this.window_.Height = value; }
      }

      public void Close() => this.window_.Close();
    }
  }
}