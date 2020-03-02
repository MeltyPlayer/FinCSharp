namespace fin.app.scene {
  using fin.events;

  public class SceneInitTickEvent : BEvent {
    public IApp App { get; }

    public SceneInitTickEvent(IApp app) {
      this.App = app;
    }
  }

  public class SceneEndTickEvent : BEvent {}
}