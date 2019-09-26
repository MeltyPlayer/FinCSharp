using System;
using System.Collections.Generic;
using System.Linq;
using fin.generic;

namespace fin.app.phase {

  public interface ITickHandler {

    void Tick(params object[] phaseDatas);
  }

  public class TickHandler : ITickHandler {
    private readonly Dictionary<Type, ISet<IPhaseHandler>> dataTypeToHandlerSetMap_;

    public TickHandler() {
      this.dataTypeToHandlerSetMap_ = new Dictionary<Type, ISet<IPhaseHandler>>();
    }

    // TODO: Disposal

    public void Tick(params object[] datas) {
      var allHandledDataTypes = this.dataTypeToHandlerSetMap_.Keys;

      foreach (var data in datas) {
        var providedDataType = data.GetType();

        // TODO: Make sure these are in a good order.
        // TODO: Memoize these.
        var possibleDataTypes = new HashSet<Type>();
        possibleDataTypes.UnionWith(TypeUtil.GetAllBaseTypes(providedDataType));
        possibleDataTypes.UnionWith(TypeUtil.GetAllInterfaces(providedDataType));
        possibleDataTypes.Add(providedDataType);

        // TODO: Look this up in a cheap way.
        var handledDataTypes = possibleDataTypes.Intersect(allHandledDataTypes);

        foreach (var handledDataType in handledDataTypes) {
          // TODO: Memoize this in the map.
          var phaseHandlerType = typeof(IPhaseHandler<>).MakeGenericType(new[] { handledDataType });
          var onPhase = phaseHandlerType.GetMethod("OnPhase");

          var dataArray = new[] { data };

          var untypedHandlers = this.dataTypeToHandlerSetMap_[handledDataType];
          foreach (var untypedHandler in untypedHandlers) {
            onPhase!.Invoke(untypedHandler, dataArray);
          }
        }
      }
    }

    public void AddHandler(IPhaseHandler handler) {
      // TODO: Memoize this based on the type.
      var handlerTypes = TypeUtil.GetImplementationsOfGenericInterface(handler, typeof(IPhaseHandler<>));
      var dataTypes = handlerTypes.Select(t => t.GetGenericArguments()[0]).ToArray();

      foreach (var dataType in dataTypes) {
        if (!this.dataTypeToHandlerSetMap_.TryGetValue(dataType, out ISet<IPhaseHandler>? handlers)) {
          this.dataTypeToHandlerSetMap_[dataType] = handlers = new HashSet<IPhaseHandler>();
        }
        handlers!.Add(handler);
      }
    }

    public void AddHandlers(params IPhaseHandler[] handlers) {
      Array.ForEach(handlers, h => this.AddHandler(h));
    }
  }
}