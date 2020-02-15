﻿using System;
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

      private class RelayStream : RelayStreamImpl<Event, Action<Event>, EventSubscription> {
        public RelayStream(EventRelay parent, SafeType<Event> eventType) :
          base(parent, eventType) { }

        protected override void RemoveFromParent()
          => this.parent_.voidRelays_.Remove(this.eventType_);

        protected override Action<Event> GenerateConvertSourceToThisEmit()
          => eventType => this.parent_.Emit(eventType);

        protected override EventSubscription SubscribeToSource(IEventSource source, Action<Event> handler)
          => (source.AddListener(this.parent_.listener_, this.eventType_, handler) as EventSubscription)!;

        protected override EventSubscription AddListenerToEmitter(IEventListener listener, Action<Event> handler)
          => (this.parent_.emitter_.AddListener(listener, this.eventType_, handler) as EventSubscription)!;

        protected override Action<Event> GetActionFromSubscription(EventSubscription subscription)
          => subscription.Handler;

        protected override IContractPointer<IEventSubscription> GetContractFromSubscription(EventSubscription subscription)
          => subscription.Contract!;
      }

      private class RelayStream<T> : RelayStreamImpl<Event<T>, Action<Event<T>, T>, EventSubscription<T>> {
        public RelayStream(EventRelay parent, SafeType<Event<T>> eventType) :
          base(parent, eventType) { }

        protected override void RemoveFromParent()
          => this.parent_.tRelays_.Remove(this.genericEventType_);

        protected override Action<Event<T>, T> GenerateConvertSourceToThisEmit()
          => (eventType, value) => this.parent_.Emit(eventType, value);

        protected override EventSubscription<T> SubscribeToSource(IEventSource source, Action<Event<T>, T> handler)
          => (source.AddListener(this.parent_.listener_, this.eventType_, handler) as EventSubscription<T>)!;

        protected override EventSubscription<T> AddListenerToEmitter(IEventListener listener, Action<Event<T>, T> handler)
          => (this.parent_.emitter_.AddListener(listener, this.eventType_, handler) as EventSubscription<T>)!;

        protected override Action<Event<T>, T> GetActionFromSubscription(EventSubscription<T> subscription)
          => subscription.Handler;

        protected override IContractPointer<IEventSubscription> GetContractFromSubscription(EventSubscription<T> subscription)
          => subscription.Contract!;
      }

      private abstract class RelayStreamImpl<TEvent, TAction, TSubscription> : IRelayStream where TEvent : IEvent where TAction : Delegate where TSubscription : IEventSubscription {
        protected readonly EventRelay parent_;
        protected readonly SafeType<IEvent> genericEventType_;
        protected readonly SafeType<TEvent> eventType_;
        protected readonly TAction convertSourceToThisEmit_;

        private readonly ConcurrentDictionary<IEventSource, TSubscription> sources_ = new ConcurrentDictionary<IEventSource, TSubscription>();
        private ISuperContract? sourcesSuperContract_;

        private readonly ConcurrentDictionary<IEventListener, ISet<TSubscription>> listeners_ = new ConcurrentDictionary<IEventListener, ISet<TSubscription>>();
        private ISuperContract? listenersSuperContract_;

        public RelayStreamImpl(EventRelay parent, SafeType<TEvent> eventType) {
          this.parent_ = parent;
          this.convertSourceToThisEmit_ = this.GenerateConvertSourceToThisEmit();
          this.genericEventType_ = new SafeType<IEvent>(eventType.Value);
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