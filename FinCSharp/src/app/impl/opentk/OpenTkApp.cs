using fin.app.node;
using fin.function;
using fin.graphics.common.impl.opentk;
using fin.input;
using fin.settings;

namespace fin.app.impl.opentk {

  public partial class OpenTkApp : BApp {
    private readonly RecurrentCaller ticker_;

    private readonly OpenTkGraphics g_ = new OpenTkGraphics();

    private readonly OpenTkWindow window_;
    private readonly KeyStateDictionary ksd_ = new KeyStateDictionary();

    public OpenTkApp() {
      var settings = Settings.Load();

      this.window_ = new OpenTkWindow(this, settings.Resolution.Width, settings.Resolution.Height, this.ksd_, this.CloseApp_);
      this.ticker_ = RecurrentCaller.FromFrequency(settings.Framerate, this.Tick_);
    }

    private void CloseApp_() {
      this.ticker_.Stop();
      this.Discard();
    }

    public override void Launch(BScene scene) {
      this.ticker_.Start();

      //this.window_.Title = $"SimpleGame ({(int)this.ticker_.ActualFrequency})";
    }

    private void Tick_() {
      // StartTickPhase
      // DisposePhase
      // EndTickPhase
      /*SCENE_INITIALIZATION = 1,
        ACTOR_MANAGEMENT = 2,
        RESOURCE_LOADING = 3,
        NET = 4,
        CONTROL = 5,
        // First apply velocity, then change in acceleration.
        PHYSICS = 6,
        COLLISION = 7,
        ANIMATION = 8,
        RENDER = 9,*/

      // TODO: Extract this out into a separate class.
      this.Emit(new StartTickEvent());
      this.Emit(new SceneInitEvent());
      this.Emit(new RenderEvent(this.g_));
    }
  }
}