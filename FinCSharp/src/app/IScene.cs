using fin.app.phase;

namespace fin.app {

  public class SceneInit {
  }

  public abstract class Scene : ReflectivePhaseHandler, IReflectivePhaseHandler<SceneInit> {

    public abstract void OnPhase(SceneInit phaseData);
  }

  public class SimpleScene : Scene {

    public override void OnPhase(SceneInit phaseData) {
    }
  }
}