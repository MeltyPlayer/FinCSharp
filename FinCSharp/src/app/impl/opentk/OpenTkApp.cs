using fin.app.phase;
using fin.function;
using fin.graphics.common;
using fin.graphics.common.impl.opentk;
using fin.settings;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace fin.app.impl.opentk {

  public partial class OpenTkApp : IApp {
    private readonly OpenTkWindow window_;
    private readonly RecurrentCaller ticker_;

    private readonly DisposePhase d_ = new DisposePhase();
    private readonly OpenTkGraphics g_ = new OpenTkGraphics();

    private readonly TickHandler tickHandler_ = new TickHandler();

    public OpenTkApp() {
      var settings = Settings.Load();
      this.window_ = new OpenTkWindow(settings.Resolution.Width, settings.Resolution.Height, this.Dispose);
      this.ticker_ = RecurrentCaller.FromFrequency(settings.Framerate, this.Tick_);
      this.OnDisposeEvent += this.ticker_.Stop;

      this.tickHandler_.AddHandler(this.window_);
    }

    public override void Launch(Scene scene) {
      GL.Enable(EnableCap.DepthTest);
      GL.ClearColor(Color4.Cyan);

      this.ticker_.Start();

      //this.window_.Title = $"SimpleGame ({(int)this.ticker_.ActualFrequency})";
    }

    private void Tick_() => this.tickHandler_.Tick(
      this.d_,
      this.g_);
  }
}