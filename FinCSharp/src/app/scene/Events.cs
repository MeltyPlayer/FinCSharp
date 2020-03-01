namespace fin.app.scene {
  using fin.events;

  using node;

  using window;

  public class SceneInitTickEvent : BEvent {
    public IInstantiator Instantiator { get; }
    public IWindowManager WindowManager { get; }

    public SceneInitTickEvent(IInstantiator instantiator,
                              IWindowManager windowManager) {
      this.Instantiator = instantiator;
      this.WindowManager = windowManager;
    }
  }

  public class SceneEndTickEvent : BEvent {}
}