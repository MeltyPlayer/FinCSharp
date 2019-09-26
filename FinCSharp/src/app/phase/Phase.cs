using System;
using System.Collections.Generic;
using System.Linq;

using fin.generic;
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

  public interface IPhaseHandler { }

  public interface IPhaseHandler<in TPhaseData> : IPhaseHandler {

    void OnPhase(TPhaseData phaseData);
  }

  public interface IControlHandler : IPhaseHandler<IInput> { }

  public interface IRenderHandler : IPhaseHandler<IGraphics> { }
}