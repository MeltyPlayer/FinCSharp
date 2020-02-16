using System;
using System.Linq;

using fin.app.events;
using fin.assert;
using fin.data.graph;
using fin.events;
using fin.type;

namespace fin.app.node {

  /// <summary>
  ///   Internal implementation of a root app node pulled out to make protected
  ///   methods easier to test. Will be wrapped by BRootAppNode.
  /// </summary>
  public class RootAppNodeInternal : BAppNodeInternal {
    public RootAppNodeInternal() : base(null) {
    }
  }

  /// <summary>
  ///   Internal implementation of a regular app node pulled out to make
  ///   protected methods easier to test. Will be wrapped by BAppNode.
  /// </summary>
  public class AppNodeInternal : BAppNodeInternal {
    public AppNodeInternal(BAppNodeInternal parent) : base(parent) {
    }

    public BAppNodeInternal Parent {
      get => this.ParentImpl!;
      set => this.ParentImpl = value;
    }
  }

  /// <summary>
  ///   Do NOT directly inherit from this.
  /// </summary>
  // TODO: Better way to enforce this???
  public abstract class BAppNodeInternal : IForOnTickMethod {
    private readonly Node<BAppNodeInternal> node_;

    private readonly IEventListener listener_ = IEventFactory.Instance.NewListener();

    private readonly IEventRelay downwardRelay_ = IEventFactory.Instance.NewRelay();

    public delegate void OnDestroyHandler();

    public event OnDestroyHandler OnDestroy = delegate { };

    public BAppNodeInternal(BAppNodeInternal? parent) {
      this.node_ = new Node<BAppNodeInternal>(this);
      this.ParentImpl = parent;

      this.OnDestroy += this.UnsubscribeAll_;

      OnTickAttribute.SniffAndAddMethods(this, this);
    }

    // TODO: This will be invalid for root nodes.
    public bool IsDestroyed => this.node_.IncomingNodes.Count() == 0;

    // TODO: Schedule for destruction, handle this at a later time.
    public void Destroy() {
      Asserts.False(this.IsDestroyed);

      this.node_.RemoveAllIncoming();
      this.OnDestroy();

      this.ForChildren(child => child.Destroy());
    }

    // Hierarchy logic.
    protected BAppNodeInternal? ParentImpl {
      get {
        var incoming = this.node_.IncomingNodes;

        if (incoming.Count() > 0) {
          return incoming.Single().Value;
        }
        return null;
      }
      set {
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

    public void ForChildren(Action<BAppNodeInternal> handler) {
      foreach (var childNode in this.node_.OutgoingNodes) {
        var child = childNode.Value;
        handler(child);
      }
    }

    // Event logic.
    public void Emit<TEvent>(TEvent evt) where TEvent : IEvent => this.downwardRelay_.Emit(evt);
    public IEventSubscription? OnTick<TEvent>(SafeType<TEvent> eventType, Action<TEvent> handler) where TEvent : IEvent {
      if (this.ParentImpl != null) {
        return this.ParentImpl.downwardRelay_.AddListener(this.listener_, eventType, handler);
      }
      return null;
    }
    public void ForOnTickMethod<TEvent>(SafeType<TEvent> eventType, Action<TEvent> handler) where TEvent : IEvent
      => this.OnTick(eventType, handler);

    private void UnsubscribeAll_() {
      this.listener_.UnsubscribeAll();
      this.downwardRelay_.Destroy();
    }
  }
}