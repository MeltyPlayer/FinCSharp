using System;
using System.Linq;

using fin.assert;
using fin.data.graph;
using fin.events;

namespace fin.app {

  public class AppNode {
    private readonly Node<AppNode> node_;
    //private readonly EventListener listener_ = new EventListener();

    //private readonly EventEmitter emitter_ = new EventEmitter();

    public delegate void OnDestroyHandler();

    public event OnDestroyHandler OnDestroy = delegate { };

    public AppNode(AppNode parent) {
      this.node_ = new Node<AppNode>(this);
      parent.AddChild(this);

      //this.OnDestroy += this.UnsubscribeAll_;
    }

    public bool IsDestroyed => this.node_.IncomingNodes.Count() == 0;

    // TODO: Schedule for destruction, handle this at a later time.
    public void Destroy() {
      Asserts.False(this.IsDestroyed);

      this.node_.RemoveAllIncoming();
      this.OnDestroy();

      this.ForChildren((child) => child.Destroy());
    }

    // Hierarchy logic.
    public AppNode Parent => this.node_.IncomingNodes.Single<INode<AppNode>>().Value;

    public bool AddChild(AppNode other) {
      Asserts.Different(this, other);
      return this.node_.AddOutgoing(other.node_);
    }

    public void ForChildren(Action<AppNode> handler) {
      foreach (var childNode in this.node_.OutgoingNodes) {
        var child = childNode.Value;
        handler(child);
      }
    }

    // Event logic.
    /*public IEventSubscription<T> On<T>(EventType<T> eventType, Action<T> handler) =>
      this.handler_.ListenTo(this.Parent.emitter_, eventType, handler);

    public IEventSubscription<T> OnEventFromSource<T>(IEventSource source, EventType<T> eventType, Action<T> handler) =>
      this.handler_.ListenTo(source, eventType, handler);*/

    /*private void UnsubscribeAll_() {
      this.listener_.UnsubscribeAll();
      this.emitter_.UnsubscribeAll();
    }

    /*public IEventSubscription<T> Subscribe<T>(IEventListener child, EventType<T> eventType, Action<T> handler) {
    }*/

    private class EventNode {
      // TODO:
    }
  }
}