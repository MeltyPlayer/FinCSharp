using System;

using fin.app.events;
using fin.assert.fluent;
using fin.data.collections.set;
using fin.discardable;
using fin.events;
using fin.type;

namespace fin.app.node.impl {
  // TODO: Make this internal.
  public sealed partial class InstantiatorImpl : IInstantiator {
    private sealed class AppNodeImpl : IAppNode {
      private readonly IDiscardableNode discardableImpl_;
      private AppNodeImpl? parent_;

      private readonly IFinSet<IComponent> components_ =
          new FinHashSet<IComponent>();

      private readonly IEventListener listener_ =
          IEventFactory.INSTANCE.NewListener();

      private readonly IEventRelay downwardRelay_ =
          IEventFactory.INSTANCE.NewRelay();

      private readonly ForOnTickMethodImpl forOnTickMethod_;

      private class ForOnTickMethodImpl : IForOnTickMethod {
        private readonly AppNodeImpl parent_;

        public ForOnTickMethodImpl(AppNodeImpl parent) {
          this.parent_ = parent;
        }

        public void ForOnTickMethod<TEvent>(
            SafeType<TEvent> eventType,
            Action<TEvent> handler) where TEvent : IEvent
          => this.parent_.OnTick(eventType, handler);
      }

      public AppNodeImpl(IDiscardableNode parentDiscardableNode) {
        this.discardableImpl_ = parentDiscardableNode.CreateChild();
        this.discardableImpl_.OnDiscard += _ => this.Discard_();

        this.forOnTickMethod_ = new ForOnTickMethodImpl(this);
      }

      public AppNodeImpl(AppNodeImpl parentAppNode) {
        this.discardableImpl_ = parentAppNode.discardableImpl_.CreateChild();
        this.discardableImpl_.OnDiscard += _ => this.Discard_();

        this.SetParent(parentAppNode);

        this.forOnTickMethod_ = new ForOnTickMethodImpl(this);
      }

      public event IDiscardableNode.OnDiscardHandler OnDiscard {
        add => this.discardableImpl_.OnDiscard += value;
        remove => this.discardableImpl_.OnDiscard -= value;
      }

      public bool IsDiscarded => this.discardableImpl_.IsDiscarded;

      private void Discard_() {
        this.listener_.UnsubscribeAll();
        this.downwardRelay_.Destroy();
      }

      public void SetParent(IAppNode parent)
        => this.SetParent_(Expect.That(parent).AsA<AppNodeImpl>());

      private void SetParent_(AppNodeImpl parent) {
        if (this.IsDiscarded) {
          return;
        }
        // TODO: Check no loops!
        // TODO: Leverage discardableImpl hierarchy??

        // Clean up old parent
        if (this.parent_ != null) {
          this.downwardRelay_.RemoveRelaySource(this.parent_.downwardRelay_);
        }

        // Set new parent
        this.parent_ = parent;
        this.downwardRelay_.AddRelaySource(this.parent_.downwardRelay_);
        this.discardableImpl_.SetParent(this.parent_.discardableImpl_);
      }

      /**
       * Component logic
       */
      public bool AddComponent(IComponent component) {
        // TODO: Assert that component is not discarded.

        if (this.components_.Add(component)) {
          // TODO: Remove these in the Remove counterpart.
          OnTickAttribute.SniffAndAddMethods(component,
                                             this.forOnTickMethod_);
          return true;
        }

        return false;
      }

      public bool RemoveComponent(IComponent component) {
        if (this.components_.Remove(component)) {
          // TODO: Remove methods.
          return true;
        }

        return false;
      }

      /**
       * Event logic
       */
      public Action<TEvent> CompileEmit<TEvent>() where TEvent : IEvent
        => this.downwardRelay_.CompileEmit<TEvent>();

      public IEventSubscription? OnTick<TEvent>(
          SafeType<TEvent> eventType,
          Action<TEvent> handler) where TEvent : IEvent {
        if (this.IsDiscarded) {
          return null;
        }

        return this.parent_?.downwardRelay_.AddListener(
            this.listener_,
            eventType,
            handler);
      }
    }
  }
}