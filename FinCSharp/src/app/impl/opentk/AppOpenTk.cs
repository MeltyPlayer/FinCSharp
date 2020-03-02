using fin.app.node;
using fin.app.node.impl;
using fin.app.scene;
using fin.app.window;
using fin.function;
using fin.graphics.impl.opentk;
using fin.input;
using fin.settings;
using fin.input.impl.opentk;

namespace fin.app.impl.opentk {
  public partial class AppOpenTk : IApp {
    private readonly IRecurrentCaller ticker_;

    private readonly IRootAppNode root_;
    private readonly ISceneManager sceneManager_ = new SceneManagerImpl();

    private readonly GraphicsOpenTk g_ = new GraphicsOpenTk();

    public IInput Input => this.input_;
    private readonly InputOpenTk input_ = new InputOpenTk();

    public IInstantiator Instantiator { get; } = new InstantiatorImpl();

    public IWindowManager WindowManager => this.windowManager_;
    private readonly WindowManagerOpenTk windowManager_;

    public AppOpenTk() {
      var settings = Settings.Load();

      this.windowManager_ = new WindowManagerOpenTk(this);
      this.root_ = this.Instantiator.NewRoot();

      this.ticker_ =
          RecurrentCaller.FromFrequency(settings.Framerate, this.Tick_);
    }

    private void ScheduleCloseApp_() {
      if (this.ticker_.IsRunning) {
        this.ticker_.Pause().Then(() => this.root_.Discard());
      }
    }

    public void Launch(IScene scene) {
      this.sceneManager_.ScheduleGoToScene(scene);
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
      this.root_.Emit(new StartTickEvent());

      this.windowManager_.ProcessEvents();
      this.input_.ButtonManager.HandleTransitions();
      if (this.input_.Keyboard[KeyId.ESC].IsPressed) {
        this.ScheduleCloseApp_();
      }

      // TODO: Should these args be passed in?
      // TODO: Event triggering should probably be limited to this class.
      // TODO: Instantiator should probably be pre-scoped to the root already.
      this.sceneManager_.ExitSceneIfScheduled(this.root_);
      this.sceneManager_.EnterSceneIfScheduled(this.root_, this);

      this.root_.Emit(new TriggerRenderViewsTickEvent(this.g_));
    }
  }
}