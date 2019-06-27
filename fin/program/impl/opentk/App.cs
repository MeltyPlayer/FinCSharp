using fin.function;
using fin.settings;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;
using System;

namespace fin.program.impl.opentk {
  public class App : IDisposable {
    private Settings settings_ = Settings.Load();
    private Window window_;
    private RecurrentCaller ticker_;
    private GraphicsContext glContext_;

    public void Dispose() {
      ticker_.Stop();
    }

    public void Launch() {
      window_ = new Window(settings_.resolution.width, settings_.resolution.height, OnEnd_);

      IWindowInfo windowInfo = window_.window.WindowInfo;
      glContext_ = new GraphicsContext(GraphicsMode.Default, windowInfo, 1, 0, GraphicsContextFlags.Default);
      glContext_.MakeCurrent(windowInfo);
      (glContext_ as IGraphicsContextInternal).LoadAll();

      GL.Enable(EnableCap.DepthTest);
      GL.ClearColor(Color4.Cyan);

      ticker_ = new RecurrentCaller(settings_.framerate, Tick_);
      ticker_.Start();
    }

    private void Tick_() {
      glContext_.MakeCurrent(window_.window.WindowInfo);

      GL.Viewport(0, 0, settings_.resolution.width, settings_.resolution.height);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      glContext_.SwapBuffers();
    }

    private void OnEnd_() {
      Dispose();
    }

    private class Window : IWindow {
      public readonly INativeWindow window;

      // TODO: Prevent instantiation by anyone other than game.
      public Window(int width, int height, Action onClose) {
        window = new NativeWindow(width, height, "SimpleGame", GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default);
        window.Visible = true;

        window.Closed += (s, e) => onClose();
      }

      public int width {
        get { return window.Width; }
        set { window.Width = value; }
      }
      public int height {
        get { return window.Height; }
        set { window.Height = value; }
      }

      public void Close() {
        window.Close();
      }
    }
  }
}
