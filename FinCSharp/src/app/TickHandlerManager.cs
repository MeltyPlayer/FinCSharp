using System;
using System.Collections.Generic;

namespace fin.app {
  public interface IPhaseManager {
    TickPhase Phase { get; }
  }

  public interface IPhaseHandler {}

  public interface IPhaseHandler<in TPhaseManager> : IPhaseHandler
    where TPhaseManager : IPhaseManager {
    void OnPhase(TPhaseManager phaseManager);
  }

  public interface ITickHandlerManager {
    void OnPhase<TPhaseManager>(TPhaseManager phaseManager)
      where TPhaseManager : IPhaseManager;

    void AddPhaseHandler<TPhaseManager>(
      IPhaseHandler<TPhaseManager> phaseHandler)
      where TPhaseManager : IPhaseManager;
  }

  public class TickHandlerManager : ITickHandlerManager, IDisposable {
    private readonly Dictionary<TickPhase, ISet<IPhaseHandler>>
      tickHandlers_ = null; // new Dictionary<TickPhase, ISet<IPhaseHandler>>();

    // TODO: Use proper dispose?
    public void Dispose() {
      foreach (var phaseHandlers in this.tickHandlers_.Values) {
        phaseHandlers.Clear();
      }
      this.tickHandlers_.Clear();
    }

    public void OnPhase<TPhaseManager>(TPhaseManager phaseManager)
      where TPhaseManager : IPhaseManager {
      var phase = phaseManager.Phase;
      foreach (var untypedPhaseHandler in this.tickHandlers_[phase]) {
        var phaseHandler = untypedPhaseHandler as IPhaseHandler<TPhaseManager>;
        phaseHandler.OnPhase(phaseManager);
      }
    }

    public void AddPhaseHandler<TPhaseManager>(
      IPhaseHandler<TPhaseManager> phaseHandler)
      where TPhaseManager : IPhaseManager {
      /*var phase = TPhaseManager.Phase;

      var phaseHandlers = this.tickHandlers_[phase];
      if (phaseHandlers == null) {
        phaseHandlers = new HashSet<IPhaseHandler>();
        this.tickHandlers_[phase] = phaseHandlers;
      }

      phaseHandlers.Add(phaseHandler);*/
    }
  }
}