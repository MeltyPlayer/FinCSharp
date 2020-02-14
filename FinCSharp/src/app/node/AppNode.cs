using System;
using System.Linq;

using fin.assert;
using fin.data.graph;
using fin.events;

namespace fin.app.node {

  public class AppNode {
    private readonly Node<AppNode> node_;

    private readonly IEventListener listener_ = IEventFactory.Instance.NewListener();

    private readonly IEventRelay downwardRelay_ = IEventFactory.Instance.NewRelay();

    public delegate void OnDestroyHandler();

    public event OnDestroyHandler OnDestroy = delegate { };

    public AppNode(AppNode parent) {
      this.node_ = new Node<AppNode>(this);
      this.Parent = parent;

      this.OnDestroy += this.UnsubscribeAll_;
    }

    public bool IsDestroyed => this.node_.IncomingNodes.Count() == 0;

    // TODO: Schedule for destruction, handle this at a later time.
    public void Destroy() {
      Asserts.False(this.IsDestroyed);

      this.node_.RemoveAllIncoming();
      this.OnDestroy();

      this.ForChildren(child => child.Destroy());
    }

    // Hierarchy logic.
    public AppNode Parent {
      get => this.node_.IncomingNodes.Single<INode<AppNode>>().Value;
      set {
        var newParent = value;
        Asserts.Different(this, newParent);

        var oldParent = this.Parent;
        oldParent.node_.RemoveOutgoing(this.node_);
        this.downwardRelay_.RemoveRelaySource(oldParent.downwardRelay_);

        newParent.node_.AddOutgoing(this.node_);
        this.downwardRelay_.AddRelaySource(newParent.downwardRelay_);
      }
    }

    public void ForChildren(Action<AppNode> handler) {
      foreach (var childNode in this.node_.OutgoingNodes) {
        var child = childNode.Value;
        handler(child);
      }
    }

    // Event logic.
    public IEventSubscription<T> OnTick<T>(EventType<T> eventType, Action<EventType<T>, T> handler) {
      return this.Parent.downwardRelay_.AddListener(this.listener_, eventType, handler);
    }

    public IEventSubscription OnTick(EventType eventType, Action<EventType> handler) {
      return this.Parent.downwardRelay_.AddListener(this.listener_, eventType, handler);
    }

    private void UnsubscribeAll_() {
      this.listener_.UnsubscribeAll();
      this.downwardRelay_.Destroy();
    }
  }
}