using System;
using System.Collections.Generic;
using System.Linq;
using fin.graphics.common;
using fin.input;

namespace fin.app.phase {
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

  public interface IPhaseManager {}

  public interface IPhaseHandler {}

  public interface IPhaseHandler<in TPhaseManager> : IPhaseHandler
    where TPhaseManager : IPhaseManager {
    void OnPhase(TPhaseManager manager);
  }

  public interface IControlHandler : IPhaseHandler<IInput> {}

  public interface IRenderHandler : IPhaseHandler<IGraphics> {}

  public class TickHandlerManager {
    private readonly Dictionary<IPhaseManager, Type> managerToTypeMap_;
    private readonly IList<IPhaseManager> managerSequence_;

    private readonly Dictionary<Type, ISet<IPhaseHandler>>
      managerTypeToHandlerSetMap_;

    public TickHandlerManager() {
      this.managerSequence_ = new List<IPhaseManager>();
      this.managerToTypeMap_ = new Dictionary<IPhaseManager, Type>();

      this.managerTypeToHandlerSetMap_ =
        new Dictionary<Type, ISet<IPhaseHandler>>();
    }

    // TODO: Disposal

    public void Tick() {
      foreach (var manager in this.managerSequence_) {
        this.OnPhase_(manager);
      }
    }

    private void OnPhase_<TPhaseManager>(TPhaseManager manager)
      where TPhaseManager : IPhaseManager {
      var managerType = this.managerToTypeMap_[manager];
      var untypedHandlers = this.managerTypeToHandlerSetMap_[managerType];
      var typedHandlers = untypedHandlers.Cast<IPhaseHandler<TPhaseManager>>();
      foreach (var typedHandler in typedHandlers) {
        typedHandler.OnPhase(manager);
      }
    }

    public TickHandlerManager ClearManagers() {
      this.managerToTypeMap_.Clear();
      this.managerSequence_.Clear();
      return this;
    }

    public TickHandlerManager AddManager<TPhaseManager>(TPhaseManager manager)
      where TPhaseManager : IPhaseManager {
      var managerType = typeof(TPhaseManager);
      this.managerToTypeMap_[manager] = managerType;
      this.managerSequence_.Add(manager);
      return this;
    }

    public TickHandlerManager AddHandler<TPhaseManager>(IPhaseHandler<TPhaseManager> handler)
      where TPhaseManager : IPhaseManager {
      var managerType = typeof(TPhaseManager);
      var handlers = this.managerTypeToHandlerSetMap_[managerType] ??
                     (this.managerTypeToHandlerSetMap_[managerType] =
                       new HashSet<IPhaseHandler>());
      handlers.Add(handler);
      return this;
    }
  }
}