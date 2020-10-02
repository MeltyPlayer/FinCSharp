using System;

using fin.app.node;
using fin.app.node.impl;
using fin.app.scene;
using fin.app.window;
using fin.audio;
using fin.audio.impl.opentk;
using fin.discardable;
using fin.function;
using fin.graphics.impl.opentk;
using fin.input;
using fin.input.impl.opentk;
using fin.input.keyboard;
using fin.settings;

using OpenTK.Audio.OpenAL;

namespace fin.app.impl.opentk {
  public partial class AppOpenTk : IApp {
    private readonly IRecurrentCaller ticker_;

    private readonly IAppNode root_;
    private readonly ISceneManager sceneManager_ = new SceneManagerImpl();

    private readonly GraphicsOpenTk g_ = new GraphicsOpenTk();

    public IAudio Audio { get; }

    public IInput Input => this.input_;
    private readonly InputOpenTk input_ = new InputOpenTk();

    public IInstantiator Instantiator { get; }

    public IAppWindowManager WindowManager => this.windowManager_;
    private readonly WindowManagerOpenTk windowManager_;

    private IDiscardableNode rootDiscardableNode_;

    public AppOpenTk() {
      new DiscardableNodeFactoryImpl(
          root => this.rootDiscardableNode_ = root);

      this.Audio = new AudioOpenTk(this.rootDiscardableNode_);
      this.Instantiator = new InstantiatorImpl(this.rootDiscardableNode_);
      this.windowManager_ =
          new WindowManagerOpenTk(this, this.rootDiscardableNode_);

      this.root_ = this.Instantiator.NewRoot();

      var settings = Settings.Load();
      this.ticker_ =
          RecurrentCaller.FromFrequency(settings.Framerate,
                                        this.CompileTick_());
    }

    private void ScheduleCloseApp_() {
      if (this.ticker_.IsRunning) {
        this.ticker_.Pause().Then(() => this.rootDiscardableNode_.Discard());
      }
    }

    public void Launch(IScene scene) {
      this.sceneManager_.ScheduleGoToScene(scene);
      this.ticker_.Start();
    }

    private Action CompileTick_() {
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

      var emitStartTick = this.root_.CompileEmit<StartTickEvent>();

      var emitProcessInputs = this.root_.CompileEmit<ProcessInputsEvent>();
      var emitTickPhysics = this.root_.CompileEmit<TickPhysicsEvent>();
      var emitTickCollisions = this.root_.CompileEmit<TickCollisionsEvent>();
      var emitTickAnimation = this.root_.CompileEmit<TickAnimationEvent>();

      var emitTriggerRenderViewsTick =
          this.root_.CompileEmit<TriggerRenderViewsTickEvent>();

      return () => {
        // TODO: Extract this out into a separate class.
        emitStartTick(new StartTickEvent());

        this.windowManager_.ProcessEvents();
        this.input_.ButtonManager.HandleTransitions();
        if (this.input_.Keyboard[KeyId.ESC].IsPressed) {
          this.ScheduleCloseApp_();
        }

        this.Audio.Poll();

        // TODO: Should these args be passed in?
        // TODO: Event triggering should probably be limited to this class.
        // TODO: Instantiator should probably be pre-scoped to the root already.
        this.sceneManager_.ExitSceneIfScheduled(this.root_);
        this.sceneManager_.EnterSceneIfScheduled(this.root_, this);

        emitProcessInputs(new ProcessInputsEvent(this.input_));
        emitTickPhysics(new TickPhysicsEvent());
        emitTickCollisions(new TickCollisionsEvent());
        emitTickAnimation(new TickAnimationEvent());

        emitTriggerRenderViewsTick(new TriggerRenderViewsTickEvent(this.g_));

        foreach (var window in this.windowManager_.Windows) {
          window.Title = $"SimpleGame ({(int) this.ticker_.ActualFrequency})";
        }
      };
    }
  }
}