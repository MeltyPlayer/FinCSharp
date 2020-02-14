using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using fin.pointer.contract;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    private sealed partial class EventRelayer : IEventRelay {

      private interface IRelayStream {

        void Destroy();

        bool AddSource(IEventSource source);

        bool RemoveSource(IEventSource source);
      }

      private class RelayStream : RelayStreamImpl<EventType, Action<EventType>, EventSubscription> {

        public RelayStream(EventRelayer parent, EventType eventType) :
          base(parent, eventType) { }

        protected override void RemoveFromParent()
          => this.parent_.voidRelays_.Remove(this.eventType_);

        protected override Action<EventType> GenerateConvertSourceToThisEmit()
          => eventType => this.parent_.Emit(eventType);

        protected override EventSubscription SubscribeToSource(IEventSource source, Action<EventType> handler)
          => (this.parent_.listener_.SubscribeTo(source, this.eventType_, handler) as EventSubscription)!;

        protected override EventSubscription AddListenerToEmitter(IEventListener listener, Action<EventType> handler)
          => (this.parent_.emitter_.AddListener(listener, this.eventType_, handler) as EventSubscription)!;

        protected override Action<EventType> GetActionFromSubscription(EventSubscription subscription)
          => subscription.Handler;

        protected override IContractPointer<IEventSubscription> GetContractFromSubscription(EventSubscription subscription)
          => subscription.Contract!;
      }

      private class RelayStream<T> : RelayStreamImpl<EventType<T>, Action<EventType<T>, T>, EventSubscription<T>> {

        public RelayStream(EventRelayer parent, EventType<T> eventType) :
          base(parent, eventType) { }

        protected override void RemoveFromParent()
          => this.parent_.tRelays_.Remove(this.eventType_);

        protected override Action<EventType<T>, T> GenerateConvertSourceToThisEmit()
          => (eventType, value) => this.parent_.Emit(eventType, value);

        protected override EventSubscription<T> SubscribeToSource(IEventSource source, Action<EventType<T>, T> handler)
          => (this.parent_.listener_.SubscribeTo(source, this.eventType_, handler) as EventSubscription<T>)!;

        protected override EventSubscription<T> AddListenerToEmitter(IEventListener listener, Action<EventType<T>, T> handler)
          => (this.parent_.emitter_.AddListener(listener, this.eventType_, handler) as EventSubscription<T>)!;

        protected override Action<EventType<T>, T> GetActionFromSubscription(EventSubscription<T> subscription)
          => subscription.Handler;

        protected override IContractPointer<IEventSubscription> GetContractFromSubscription(EventSubscription<T> subscription)
          => subscription.Contract!;
      }

      private abstract class RelayStreamImpl<TEventType, TAction, TSubscription> : IRelayStream where TEventType : IEventType where TAction : Delegate where TSubscription : IEventSubscription {
        protected readonly EventRelayer parent_;
        protected readonly TEventType eventType_;
        protected readonly TAction convertSourceToThisEmit_;

        private readonly ConcurrentDictionary<IEventSource, TSubscription> sources_ = new ConcurrentDictionary<IEventSource, TSubscription>();
        private ISuperContract? sourcesSuperContract_;

        private readonly ConcurrentDictionary<IEventListener, ISet<TSubscription>> listeners_ = new ConcurrentDictionary<IEventListener, ISet<TSubscription>>();
        private ISuperContract? listenersSuperContract_;

        public RelayStreamImpl(EventRelayer parent, TEventType eventType) {
          this.parent_ = parent;
          this.convertSourceToThisEmit_ = this.GenerateConvertSourceToThisEmit();
          this.eventType_ = eventType;

          foreach (var source in this.parent_.relaySources_) {
            this.AddSource(source);
          }
        }

        public void Destroy() {
          this.listenersSuperContract_!.Break();
        }

        protected abstract void RemoveFromParent();

        protected abstract TAction GenerateConvertSourceToThisEmit();

        protected abstract TSubscription SubscribeToSource(IEventSource source, TAction handler);

        protected abstract TSubscription AddListenerToEmitter(IEventListener listener, TAction handler);

        protected abstract TAction GetActionFromSubscription(TSubscription subscription);

        protected abstract IContractPointer<IEventSubscription> GetContractFromSubscription(TSubscription subscription);

        public bool AddSource(IEventSource source) {
          if (this.sources_.ContainsKey(source)) {
            return false;
          }

          var subscription = this.SubscribeToSource(source, this.convertSourceToThisEmit_);
          this.sources_.TryAdd(source, subscription);

          var contract = this.GetContractFromSubscription(subscription);
          if (this.sourcesSuperContract_ == null || !this.sourcesSuperContract_.IsActive) {
            this.sourcesSuperContract_ = IContractFactory.Instance.NewSuperContract(contract);
          } else {
            this.sourcesSuperContract_.Add(contract);
          }

          contract.OnBreak += _ => this.RemoveSource(source);
          return true;
        }

        public bool RemoveSource(IEventSource source) {
          if (this.sources_.Remove(source, out TSubscription subscription)) {
            var contract = this.GetContractFromSubscription(subscription);
            contract.Break();
            return true;
          }
          return false;
        }

        public TSubscription AddListener(IEventListener listener, TAction handler) {
          if (this.listeners_.TryGetValue(listener, out ISet<TSubscription>? subscriptions)) {
            foreach (var s in subscriptions!) {
              if (this.GetActionFromSubscription(s) == handler) {
                return s;
              }
            }
          } else {
            subscriptions = new HashSet<TSubscription>();
            this.listeners_.TryAdd(listener, subscriptions);
          }

          var subscription = this.AddListenerToEmitter(listener, handler);
          subscriptions.Add(subscription);

          var contract = this.GetContractFromSubscription(subscription);
          if (this.listenersSuperContract_ == null) {
            this.listenersSuperContract_ = IContractFactory.Instance.NewSuperContract(contract);
            this.listenersSuperContract_.OnBreak += _ => {
              if (this.sourcesSuperContract_ != null) {
                this.sourcesSuperContract_.Break();
              }
              this.RemoveFromParent();
            };
          } else {
            this.listenersSuperContract_.Add(contract);
          }

          contract.OnBreak += _ => this.RemoveListener(listener);
          return subscription;
        }

        public void RemoveListener(IEventListener listener) {
          if (this.listeners_.Remove(listener, out ISet<TSubscription>? subscriptions)) {
            foreach (var subscription in subscriptions) {
              var contract = this.GetContractFromSubscription(subscription!);
              contract.Break();
            }
          }
        }
      }
    }
  }
}