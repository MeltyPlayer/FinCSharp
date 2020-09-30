// TODO: How much overhead does this all introduce?

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using fin.data.collections.dictionary;
using fin.data.collections.set;
using fin.pointer.contract;
using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory : IEventFactory {
    private abstract class EventOwner {
      private class
          EventContractDictionary : IContractPointerSet<IEventSubscription> {
        // TODO: Higher overhead than should be necessary.
        private readonly IFinSet<IContractPointer<IEventSubscription>>
            contracts_ = new FinHashSet<IContractPointer<IEventSubscription>>();

        private readonly
            IMultiDictionary<SafeType<IEvent>,
                IContractPointer<IEventSubscription>> sets_ =
                new MultiDictionary<SafeType<IEvent>,
                    IContractPointer<IEventSubscription>>();

        public IEnumerable<IContractPointer<IEventSubscription>> Contracts =>
            this.contracts_;

        public int Count => this.contracts_.Count;

        public IEnumerable<IContractPointer<IEventSubscription>>? Get(
            SafeType<IEvent> genericEventType)
          => this.sets_.TryGet(genericEventType);

        public bool Add(IContractPointer<IEventSubscription> contract) {
          if (this.contracts_.Add(contract)) {
            var genericEventType = contract.Value.EventType;
            this.sets_.Add(genericEventType, contract);
            return true;
          }

          return false;
        }

        public bool Remove(IContractPointer<IEventSubscription> contract) {
          var genericEventType = contract.Value.EventType;
          if (this.sets_.Remove(genericEventType, contract)) {
            if (this.Get(genericEventType) == null) {
              this.contracts_.Remove(contract);
            }
            return true;
          }

          return false;
        }

        public void ClearAndBreak(
            Action<IContractPointer<IEventSubscription>> breakHandler) {
          while (this.contracts_.Count > 0) {
            breakHandler(this.contracts_.First());
          }

          this.sets_.Clear();
        }
      }

      private readonly EventContractDictionary set_ =
          new EventContractDictionary();

      protected readonly IWeakContractPointerOwner<IEventSubscription> owner_;

      public EventOwner() {
        this.owner_ = IContractFactory.INSTANCE.NewWeakOwner(this.set_);
      }

      public IEnumerable<IContractPointer<IEventSubscription>>? Get(
          SafeType<IEvent> genericEventType) => this.set_.Get(genericEventType);

      protected EventSubscription<TEvent> CreateSubscription<TEvent>(
          IEventSource source,
          IEventListener listener,
          SafeType<TEvent> eventType,
          Action<TEvent> action) where TEvent : IEvent {
        var subscription =
            this.HasSubscription_(source, listener, eventType, action);
        if (subscription != null) {
          return subscription;
        }

        subscription =
            new EventSubscription<TEvent>(source, listener, eventType, action);
        var contract =
            (source as EventSource)!.owner_.FormClosedWith(subscription,
                                                           (listener as
                                                                EventListener)!
                                                           .owner_);
        subscription.Contract = contract;
        return subscription;
      }

      // TODO: Find a faster way to do this.
      private EventSubscription<TEvent>? HasSubscription_<TEvent>(
          IEventSource source,
          IEventListener listener,
          SafeType<TEvent> eventType,
          Action<TEvent> action) {
        var contracts = this.set_.Contracts;
        foreach (var contract in contracts) {
          if (contract.Value is EventSubscription<TEvent> subscription) {
            if (subscription.Source == source &&
                subscription.Listener == listener &&
                subscription.EventType == eventType &&
                subscription.Handler == action) {
              return subscription;
            }
          }
        }

        return null;
      }
    }
  }
}