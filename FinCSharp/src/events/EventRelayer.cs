using System.Collections.Concurrent;
using System.Collections.Generic;

using fin.pointer.contract;

namespace fin.events {
  // TODO: Benchmark this OnUnsubscribe approach compared to the
  // ContractEventSubscriptionDictionary's IContractOwner approach. That's at
  // least less memory-intensive, right?
  /*public class EventRelayerNode {
    private class RelayStream : IContractOwner<IEventSubscription> {
      private RelayStreamDictionary parent_;
      private IList<EventRelayerNode> upstreamNodes_ = new List<EventRelayerNode>();
      private IList<EventRelayerNode> downstreamNodes_ = new List<EventRelayerNode>();

      public bool IsEmpty => this.upstreamNodes_.Count == 0 && this.downstreamNodes_.Count == 0;

      public bool Join(IContractPointer<IEventSubscription> genericContract)
        => this.Get(genericContract.Value.IEventType).Join(genericContract);

      public bool Break(IContractPointer<IEventSubscription> contract) {
        var genericEventType = contract.Value.IEventType;
        if (this.contractSets_.TryGetValue(genericEventType, out object? genericContractSet)) {
          var contractSet = genericContractSet as ContractSet<IEventSubscription>;
          if (contractSet!.Break(contract)) {
            if (contractSet.UpstreamNodes.Count() == 0 || contra) {
              this.contractSets_.TryRemove(genericEventType, out object? _);
            }
            return true;
          }
        }
        return false;
      }

      public void BreakAll() {
        foreach (var genericContractSet in this.contractSets_.Values) {
          var contractSet = genericContractSet as ContractSet<IEventSubscription>;
          contractSet!.BreakAll();
        }
        this.contractSets_.Clear();
      }
    }

    private class RelayStreamDictionary : IContractOwner<IEventSubscription> {
      private EventRelayerNode parent_;

      private readonly ConcurrentDictionary<IEventType, RelayStream> relayStreams_ =
        new ConcurrentDictionary<IEventType, RelayStream>();

      public RelayStream Get(IEventType eventType)
        => this.relayStreams_.GetOrAdd(eventType, eventType => new RelayStream());

      public bool Join(IContractPointer<IEventSubscription> genericContract)
        => this.Get(genericContract.Value.IEventType).Join(genericContract);

      public bool Break(IContractPointer<IEventSubscription> contract) {
        var genericEventType = contract.Value.IEventType;
        if (this.relayStreams_.TryGetValue(genericEventType, out RelayStream? relayStream)) {
          if (relayStream!.Break(contract)) {
            if (relayStream.IsEmpty) {
              this.relayStreams_.TryRemove(genericEventType, out RelayStream? _);
            }
            return true;
          }
        }
        return false;
      }

      public void BreakAll() {
        foreach (var relayStream in this.relayStreams_.Values) {
          relayStream!.BreakAll();
        }
        this.relayStreams_.Clear();
      }
    }

    private RelayStreamDictionary relayStreams_ = new RelayStreamDictionary();

    private readonly EventListener listener_ = new EventListener();
    private readonly EventEmitter emitter_ = new EventEmitter();

    public void Destroy() {
      this.relayStreams_.BreakAll();
      this.listener_.UnsubscribeAll();
      this.emitter_.UnsubscribeAll();
    }

    public void AddUpstreamSource(EventRelayerNode parent) {
      this.relayStreams_.Join();
      if (this.parents_.Count > 0) {
        // TODO: Retarget parent subscriptions.
      }

      this.parents_.Add(parent);
    }

    public void RemoveUpstreamSource(EventRelayer parent) {
    }

    private void AddNewListenedEventType() {
    }

    private void AddNewListenedEventType() {
    }

    public void AddDownstreamListener() {
    }

    public void RemoveDownstreamListener() {
    }

    public void Emit(EventType eventType) => this.emitter_.Emit(eventType);

    public void Emit<T>(EventType<T> eventType, T value) =>
      this.emitter_.Emit(eventType, value);
  }*/
}