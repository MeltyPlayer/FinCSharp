using System;
using System.Collections.Generic;
using System.Linq;
using fin.app.events;
using fin.assert;
using fin.data.collections.set;
using fin.data.graph;
using fin.discardable;
using fin.events;
using fin.type;

namespace fin.app.node {

  // TODO: Make this internal.
  public class InstantiatorImpl : IInstantiator {
    public IRootAppNode NewRoot() => new RootAppNode();

    private sealed class RootAppNode : BAppNode, IRootAppNode {
      public RootAppNode() : base(null) {
      }

      public bool Discard() => this.discardableImpl_.Discard();
    }

    public IChildAppNode NewChild(IAppNode parent, params IComponent[] components) {
      var child = new ChildAppNode((parent as BAppNode)!);
      foreach (var component in components) {
        child.AddComponent(component);
      }
      return child;
    }

    private sealed class ChildAppNode : BAppNode, IChildAppNode {
      public ChildAppNode(BAppNode parent) : base(parent) {
      }

      public IAppNode Parent {
        get => this.ParentImpl!;
        set => this.ParentImpl = (value as BAppNode)!;
      }
    }

    public TComponent Wrap<TComponent>(IAppNode parent, TComponent component) where TComponent : IComponent {
      var child = new ChildAppNode((parent as BAppNode)!);
      child.AddComponent(component);
      return component;
    }

    /// <summary>
    ///   Do NOT directly inherit from this.
    /// </summary>
    // TODO: Better way to enforce this???
    public abstract class BAppNode : IAppNode {
      private readonly Node<BAppNode> node_;

      private readonly OrderedSet<IComponent> components_ = new OrderedSet<IComponent>();

      private readonly IEventListener listener_ = IEventFactory.Instance.NewListener();
      private readonly IEventRelay downwardRelay_ = IEventFactory.Instance.NewRelay();

      protected readonly DiscardableImpl discardableImpl_ = new DiscardableImpl();

      private readonly ForOnTickMethodImpl forOnTickMethod_;

      private class ForOnTickMethodImpl : IForOnTickMethod {
        private readonly BAppNode parent_;
        public ForOnTickMethodImpl(BAppNode parent) {
          this.parent_ = parent;
        }
        public void ForOnTickMethod<TEvent>(SafeType<TEvent> eventType, Action<TEvent> handler) where TEvent : IEvent
          => this.parent_.OnTick(eventType, handler);
      }

      public BAppNode(BAppNode? parent) {
        this.node_ = new Node<BAppNode>(this);
        this.ParentImpl = parent;

        this.discardableImpl_.OnDiscard += _ => this.Discard_();

        this.forOnTickMethod_ = new ForOnTickMethodImpl(this);
      }

      public event IEventDiscardable.OnDiscardHandler OnDiscard {
        add => this.discardableImpl_.OnDiscard += value;
        remove => this.discardableImpl_.OnDiscard -= value;
      }

      public bool IsDiscarded => this.discardableImpl_.IsDiscarded;

      public bool AddParent(IEventDiscardable parent) {
        throw new NotImplementedException();
      }

      public bool RemoveParent(IEventDiscardable parent) {
        throw new NotImplementedException();
      }

      // TODO: Schedule for destruction, handle this at a later time.
      public void Discard_() {
        this.node_.RemoveAllIncoming();

        this.listener_.UnsubscribeAll();
        this.downwardRelay_.Destroy();
      }

      // Hierarchy logic.
      protected BAppNode? ParentImpl {
        get {
          var incoming = this.node_.IncomingNodes;

          if (incoming.Count() > 0) {
            return incoming.Single().Value;
          }
          return null;
        }
        set {
          if (this.IsDiscarded) {
            return;
          }

          var oldParent = this.ParentImpl;
          if (oldParent != null) {
            oldParent.node_.RemoveOutgoing(this.node_);
            this.downwardRelay_.RemoveRelaySource(oldParent.downwardRelay_);
          }

          var newParent = value;
          if (newParent != null) {
            Asserts.Different(this, newParent);
            newParent.node_.AddOutgoing(this.node_);
            this.downwardRelay_.AddRelaySource(newParent.downwardRelay_);
          }
        }
      }

      public void ForChildren(Action<IAppNode> handler) {
        if (this.IsDiscarded) {
          return;
        }

        foreach (var childNode in this.node_.OutgoingNodes) {
          var child = childNode.Value;
          handler(child);
        }
      }

      /**
       * Component logic
       */
      public bool AddComponent(IComponent component) {
        if (component.IsDiscarded) {
          return false;
        }
        if (this.components_.Add(component)) {
          // TODO: Remove these in the Remove counterpart.
          OnTickAttribute.SniffAndAddMethods(component, this.forOnTickMethod_);
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
      public void Emit<TEvent>(TEvent evt) where TEvent : IEvent {
        if (this.IsDiscarded) {
          return;
        }
        this.downwardRelay_.Emit(evt);
      }

      public IEventSubscription? OnTick<TEvent>(SafeType<TEvent> eventType, Action<TEvent> handler) where TEvent : IEvent {
        if (this.IsDiscarded) {
          return null;
        }
        if (this.ParentImpl != null) {
          return this.ParentImpl.downwardRelay_.AddListener(this.listener_, eventType, handler);
        }
        return null;
      }
    }
  }
}