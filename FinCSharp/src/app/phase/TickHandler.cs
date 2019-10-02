using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using fin.generic;

namespace fin.app.phase {

  public interface ITickHandler : IPhaseHandler {

    void Tick(params object[] phaseDatas);
  }

  public class TickHandler : ITickHandler {
    private readonly SetDictionary<Type, IPhaseHandler> dataTypeToHandlerSetMap_;

    public TickHandler() {
      this.dataTypeToHandlerSetMap_ = new SetDictionary<Type, IPhaseHandler>();
    }

    // TODO: Disposal

    public void Tick(params object[] phaseDatas) {
      foreach (var phaseData in phaseDatas) {
        this.OnPhase(phaseData);
      }
    }

    public void OnPhase(object phaseData) {
      var providedDataType = phaseData.GetType();
      var handledDataTypes = ReflectivePhaseManager.ReflectivelyAcquireCompatibleDataTypesImpl_(this.HandledPhaseTypes, ReflectivePhaseManager.ReflectivelyAcquireAllTypesImpl_(providedDataType));

      foreach (var handledDataType in handledDataTypes) {
        var untypedHandlers = this.dataTypeToHandlerSetMap_[handledDataType];
        foreach (var untypedHandler in untypedHandlers) {
          untypedHandler.OnPhase(phaseData);
        }
      }
    }

    // TODO: Automatically handle when/if this updates.
    public IEnumerable<Type> HandledPhaseTypes => this.dataTypeToHandlerSetMap_.Keys;

    public void AddHandlers(params IPhaseHandler[] handlers) => Array.ForEach(handlers, h => this.AddHandler(h));

    public void AddHandler(IPhaseHandler handler) {
      foreach (var dataType in handler.HandledPhaseTypes) {
        this.dataTypeToHandlerSetMap_.Add(dataType, handler);
      }
    }

    public void RemoveHandlers(params IPhaseHandler[] handlers) => Array.ForEach(handlers, h => this.RemoveHandler(h));

    public void RemoveHandler(IPhaseHandler handler) {
      foreach (var dataType in handler.HandledPhaseTypes) {
        this.dataTypeToHandlerSetMap_.Remove(dataType, handler);
      }
    }
  }

  public class SetDictionary<TKey, TValue> where TKey : notnull where TValue : notnull {
    private readonly IDictionary<TKey, ISet<TValue>> setDictionary_;

    public SetDictionary() {
      this.setDictionary_ = new Dictionary<TKey, ISet<TValue>>();
    }

    public void Clear() {
      foreach (var set in this.setDictionary_.Values) {
        set.Clear();
      }
      this.setDictionary_.Clear();
    }

    public ICollection<TKey> Keys => this.setDictionary_.Keys;

    public ICollection<TValue> this[TKey key] => this.setDictionary_[key];

    public bool Add(TKey key, TValue value) {
      if (!this.setDictionary_.TryGetValue(key, out ISet<TValue>? set)) {
        this.setDictionary_[key] = set = new HashSet<TValue>();
      }
      return set!.Add(value);
    }

    public bool Remove(TKey key, TValue value) {
      bool didRemove = false;
      if (this.setDictionary_.TryGetValue(key, out ISet<TValue>? set)) {
        didRemove = set.Remove(value);
        if (didRemove && set.Count == 0) {
          this.setDictionary_.Remove(key);
        }
      }
      return didRemove;
    }
  }
}