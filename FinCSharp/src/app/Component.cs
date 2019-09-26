using fin.app.phase;

namespace fin.app {

  public abstract class Component<in TPhaseData> : IPhaseHandler<TPhaseData> {
  }
}