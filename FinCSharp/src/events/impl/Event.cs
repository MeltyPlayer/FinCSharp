// TODO: This whole damn file is a war crime.
// TODO: How much overhead does this all introduce?

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using fin.data.collections.set;
using fin.pointer.contract;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    private abstract class ContractEventOwner {

      private class EventContractDictionary : IContractSet<IEventSubscription> {
        private readonly OrderedSet<IContractPointer<IEventSubscription>> contracts_ = new OrderedSet<IContractPointer<IEventSubscription>>();

        private readonly ConcurrentDictionary<IEventType, ISet<IContractPointer<IEventSubscription>>> sets_ =
          new ConcurrentDictionary<IEventType, ISet<IContractPointer<IEventSubscription>>>();

        public int Count => this.contracts_.Count;

        public IEnumerable<IContractPointer<IEventSubscription>> Contracts => this.contracts_;

        public IEnumerable<IContractPointer<IEventSubscription>>? Get(IEventType eventType) {
          this.sets_.TryGetValue(eventType, out ISet<IContractPointer<IEventSubscription>>? set);
          return set;
        }

        public bool Add(IContractPointer<IEventSubscription> contract) {
          if (this.contracts_.Add(contract)) {
            var genericEventType = contract.Value.IEventType;
            var set = this.sets_.GetOrAdd(genericEventType, genericEventType => new HashSet<IContractPointer<IEventSubscription>>());
            set.Add(contract);
            return true;
          }
          return false;
        }

        public bool Remove(IContractPointer<IEventSubscription> contract) {
          var genericEventType = contract.Value.IEventType;
          if (this.sets_.TryGetValue(genericEventType, out ISet<IContractPointer<IEventSubscription>>? set)) {
            if (set!.Remove(contract)) {
              if (set.Count() == 0) {
                this.sets_.TryRemove(genericEventType, out ISet<IContractPointer<IEventSubscription>>? _);
                this.contracts_.Remove(contract);
              }
              return true;
            }
          }
          return false;
        }

        public void Clear(Action<IContractPointer<IEventSubscription>> breakHandler) {
          while (this.contracts_.Count > 0) {
            breakHandler(this.contracts_.First);
          }
          this.sets_.Clear();
        }
      }

      private EventContractDictionary set_ = new EventContractDictionary();
      private IWeakContractOwner<IEventSubscription> owner_;

      public ContractEventOwner() {
        this.owner_ = IContractFactory.Instance.NewWeakOwner(this.set_);
      }

      public IEnumerable<IContractPointer<IEventSubscription>>? Get(IEventType eventType) => this.set_.Get(eventType);

      protected static IContractPointer<IEventSubscription> CreateContract(IEventSource source, IEventListener listener, EventType eventType, Action action) {
        var subscription = new EventSubscription(source, listener, eventType, action);
        var contract = (source as EventSource)!.owner_.FormClosedWith(subscription, (listener as EventListener)!.owner_);
        subscription.Contract = contract;
        return contract!;
      }

      protected static IContractPointer<IEventSubscription> CreateContract<T>(IEventSource source, IEventListener listener, EventType<T> eventType, Action<T> action) {
        var subscription = new EventSubscription<T>(source, listener, eventType, action);
        var contract = (source as EventSource)!.owner_.FormClosedWith(subscription, (listener as EventListener)!.owner_);
        subscription.Contract = contract;
        return contract!;
      }

      public void UnsubscribeAll() => this.owner_.BreakAll();
    }
  }
}