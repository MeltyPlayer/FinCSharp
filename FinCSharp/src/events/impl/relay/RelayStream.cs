using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using fin.pointer.contract;
using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory : IEventFactory {
    private sealed partial class EventRelay : IEventRelay {
      private interface IRelayStream {
        void Destroy();
        bool AddSource(IEventSource source);
        bool RemoveSource(IEventSource source);
      }

      private class RelayStream<TEvent> : IRelayStream where TEvent : IEvent {
        protected readonly EventRelay parent_;
        protected readonly SafeType<TEvent> eventType_;
        protected readonly Action<TEvent> convertSourceToThisEmit_;

        private readonly ConcurrentDictionary<IEventSource, IEventSubscription>
          sources_ =
            new ConcurrentDictionary<IEventSource, IEventSubscription>();

        private ISuperContract? sourcesSuperContract_;

        private readonly
          ConcurrentDictionary<IEventListener, ISet<IEventSubscription>>
          listeners_ =
            new ConcurrentDictionary<IEventListener, ISet<IEventSubscription>
            >();

        private ISuperContract? listenersSuperContract_;

        public RelayStream(EventRelay parent, SafeType<TEvent> eventType) {
          this.parent_ = parent;
          this.convertSourceToThisEmit_ = evt => this.parent_.Emit(evt);
          this.eventType_ = eventType;

          foreach (var source in this.parent_.relaySources_) {
            this.AddSource(source);
          }
        }

        public void Destroy() {
          this.listenersSuperContract_!.Break();
        }

        public bool AddSource(IEventSource source) {
          if (this.sources_.ContainsKey(source)) {
            return false;
          }

          var subscription =
            (source.AddListener(this.parent_.listener_,
              this.eventType_,
              this.convertSourceToThisEmit_) as EventSubscription<TEvent>)!;
          this.sources_.TryAdd(source, subscription);

          var contract = subscription.Contract!;
          if (this.sourcesSuperContract_ == null ||
              !this.sourcesSuperContract_.IsActive) {
            this.sourcesSuperContract_ =
              IContractFactory.Instance.NewSuperContract(contract);
          }
          else {
            this.sourcesSuperContract_.Add(contract);
          }

          contract.OnBreak += _ => this.RemoveSource(source);
          return true;
        }

        public bool RemoveSource(IEventSource source) {
          if (this.sources_.Remove(source,
            out IEventSubscription? subscription)) {
            var contract =
              (subscription as EventSubscription<TEvent>)!.Contract!;
            contract.Break();
            return true;
          }

          return false;
        }

        public IEventSubscription AddListener(IEventListener listener,
          Action<TEvent> handler) {
          if (this.listeners_.TryGetValue(listener,
            out ISet<IEventSubscription>? subscriptions)) {
            foreach (var s in subscriptions!) {
              if ((s as EventSubscription<TEvent>)!.Handler == handler) {
                return s;
              }
            }
          }
          else {
            subscriptions = new HashSet<IEventSubscription>();
            this.listeners_.TryAdd(listener, subscriptions);
          }

          var subscription =
            (this.parent_.emitter_.AddListener(listener,
              this.eventType_,
              handler) as EventSubscription<TEvent>)!;
          subscriptions.Add(subscription);

          var contract = subscription.Contract!;
          if (this.listenersSuperContract_ == null) {
            this.listenersSuperContract_ =
              IContractFactory.Instance.NewSuperContract(contract);
            this.listenersSuperContract_.OnBreak += _ => {
              if (this.sourcesSuperContract_ != null) {
                this.sourcesSuperContract_.Break();
              }

              var genericEventType =
                new SafeType<IEvent>(this.eventType_.Value);
              this.parent_.relayStreams_.Remove(genericEventType);
            };
          }
          else {
            this.listenersSuperContract_.Add(contract);
          }

          contract.OnBreak += _ => this.RemoveListener(listener);
          return subscription;
        }

        public void RemoveListener(IEventListener listener) {
          if (this.listeners_.Remove(listener,
            out ISet<IEventSubscription>? subscriptions)) {
            foreach (var subscription in subscriptions) {
              var contract = (subscription as EventSubscription<TEvent>)!
                .Contract!;
              contract.Break();
            }
          }
        }
      }
    }
  }
}