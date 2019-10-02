using fin.app.phase;
using fin.function;
using fin.graphics.common.impl.opentk;
using fin.input;
using fin.input.impl.opentk;
using fin.settings;

namespace fin.app.impl.opentk {

  public partial class OpenTkApp : IApp {
    private readonly RecurrentCaller ticker_;

    private readonly StartTickPhase stp_ = new StartTickPhase();
    private readonly DisposePhase dp_ = new DisposePhase();
    private readonly OpenTkGraphics g_ = new OpenTkGraphics();
    private readonly EndTickPhase etp_ = new EndTickPhase();

    private readonly KeyStateDictionary ksd_ = new KeyStateDictionary();
    private readonly OpenTkWindow window_;

    private readonly TickHandler tickHandler_ = new TickHandler();

    public OpenTkApp() {
      var settings = Settings.Load();

      this.window_ = new OpenTkWindow(settings.Resolution.Width, settings.Resolution.Height, this.ksd_, this.Dispose);
      this.ticker_ = RecurrentCaller.FromFrequency(settings.Framerate, this.Tick_);
      this.OnDisposeEvent += this.ticker_.Stop;

      this.tickHandler_.AddHandlers(
        this.window_);
    }

    public override void Launch(Scene scene) {
      this.ticker_.Start();

      //this.window_.Title = $"SimpleGame ({(int)this.ticker_.ActualFrequency})";
    }

    private void Tick_() => this.tickHandler_.Tick(
      this.stp_,
      this.dp_,
      this.g_,
      this.etp_);
  }
}