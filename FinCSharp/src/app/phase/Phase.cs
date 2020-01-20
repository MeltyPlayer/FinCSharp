using System;
using System.Collections.Generic;
using System.Linq;

using fin.data.collections.set;
using fin.function;
using fin.generic;
using fin.graphics.common;
using fin.input;

using DataTypeToHandlerDictionary = System.Collections.Generic.Dictionary<System.Type, System.Reflection.MethodInfo>;

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

  public class StartTickPhase { }

  public interface IPhaseHandler {
    IEnumerable<Type> HandledPhaseTypes { get; }

    void OnPhase(object phaseData);
  }

  /*public class EventPhaseHandler : IPhaseHandler {
    private delegate void OnPhaseEventHandler(object instance);

    private Dictionary<Type, Action<object>> phaseHandlers_;

    public IEnumerable<Type> HandledPhaseTypes => throw new NotImplementedException();

    public void OnPhase(object phaseData) {
      throw new NotImplementedException();
    }
  }*/

  public class ReflectivePhaseManager : IPhaseHandler {
    private readonly DataTypeToHandlerDictionary dataTypeToHandler_;

    private readonly IReflectivePhaseHandler reflectivePhaseHandler_;

    public ReflectivePhaseManager(IReflectivePhaseHandler reflectivePhaseHandler) {
      this.reflectivePhaseHandler_ = reflectivePhaseHandler;

      this.dataTypeToHandler_ = ReflectivelyAcquireDataTypesToHandlerDictionaryImpl_.Invoke(this.reflectivePhaseHandler_.GetType());
    }

    public static MemoizedFunc<Type, DataTypeToHandlerDictionary> ReflectivelyAcquireDataTypesToHandlerDictionaryImpl_ { get; } = Memoization.MemoizeDebug((Type myType) => {
      var dataTypeToHandler = new DataTypeToHandlerDictionary();

      var handlerTypes = TypeUtil.GetImplementationsOfGenericInterface(myType, typeof(IReflectivePhaseHandler<>));
      var handledDataTypes = handlerTypes.Select(t => t.GetGenericArguments()[0]).ToArray();

      // TODO: Memoize this.
      foreach (var handledDataType in handledDataTypes) {
        var phaseHandlerType = typeof(IReflectivePhaseHandler<>).MakeGenericType(new[] { handledDataType });
        dataTypeToHandler[handledDataType] = phaseHandlerType.GetMethod("OnPhase")!;
      }

      return dataTypeToHandler;
    });

    public IEnumerable<Type> HandledPhaseTypes => this.dataTypeToHandler_.Keys;

    public void OnPhase(object phaseData) {
      /*var handledDataTypes = ReflectivelyAcquireCompatibleDataTypesImpl_.Invoke(this.HandledPhaseTypes, ReflectivelyAcquireAllTypesImpl_.Invoke(phaseData.GetType()));
      foreach (var handledDataType in handledDataTypes) {
        var dataArray = new[] { phaseData };
        var onPhase = this.dataTypeToHandler_[handledDataType];
        onPhase!.Invoke(this.reflectivePhaseHandler_, dataArray);
      }*/
    }

    public static MemoizedFunc<IEnumerable<Type>, IEnumerable<Type>, Type[]> ReflectivelyAcquireCompatibleDataTypesImpl_ { get; } =
      Memoization.MemoizeDebug((IEnumerable<Type> myHandledTypes, IEnumerable<Type> possibleDataTypes) => possibleDataTypes.Intersect(myHandledTypes).ToArray());

    public static MemoizedFunc<Type, ISet<Type>> ReflectivelyAcquireAllTypesImpl_ { get; } = Memoization.MemoizeDebug((Type t) => {
      // TODO: Make sure these are in a good order.
      ISet<Type> allTypes = new HashSet<Type>();
      allTypes.UnionWith(TypeUtil.GetAllBaseTypes(t));
      allTypes.UnionWith(TypeUtil.GetAllInterfaces(t));
      allTypes.Add(t);
      return allTypes;
    });
  }

  // TODO: Figure out how to lay out these types.
  public abstract class ReflectivePhaseHandler : IPhaseHandler, IReflectivePhaseHandler {
    private readonly ReflectivePhaseManager manager_;

    public ReflectivePhaseHandler() {
      this.manager_ = new ReflectivePhaseManager(this);
    }

    public IEnumerable<Type> HandledPhaseTypes => this.manager_.HandledPhaseTypes;

    public void OnPhase(object phaseData) => this.manager_.OnPhase(phaseData);
  }

  public interface IReflectivePhaseHandler {
  }

  public interface IReflectivePhaseHandler<in TPhaseData> : IReflectivePhaseHandler {

    void OnPhase(TPhaseData phaseData);
  }

  public interface IControlHandler : IReflectivePhaseHandler<IInput> { }

  public interface IRenderHandler : IReflectivePhaseHandler<IGraphics> { }
}