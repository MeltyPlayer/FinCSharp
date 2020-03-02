using System.Collections.Generic;

namespace fin.data.graph {
  using collections.set;

  /// <summary>
  ///   A basic implementation of a graph node.
  /// </summary>
  public class Node<T> : INode<T> {
    private readonly OrderedSet<INode<T>> incomingNodes_ = new OrderedSet<INode<T>>();
    private readonly OrderedSet<INode<T>> outgoingNodes_ = new OrderedSet<INode<T>>();

    public Node(T value) {
      this.Value = value;
    }

    public T Value { get; set; }

    public IEnumerable<INode<T>> IncomingNodes => this.incomingNodes_;

    public IEnumerable<INode<T>> OutgoingNodes => this.outgoingNodes_;

    public bool AddIncoming(INode<T> other) {
      if (this.incomingNodes_.Add(other)) {
        other.AddOutgoing(this);
        return true;
      }

      return false;
    }

    public bool AddOutgoing(INode<T> other) {
      if (this.outgoingNodes_.Add(other)) {
        other.AddIncoming(this);
        return true;
      }

      return false;
    }

    public bool RemoveIncoming(INode<T> other) {
      if (this.incomingNodes_.Remove(other)) {
        other.RemoveOutgoing(this);
        return true;
      }

      return false;
    }

    public bool RemoveOutgoing(INode<T> other) {
      if (this.outgoingNodes_.Remove(other)) {
        other.RemoveIncoming(this);
        return true;
      }

      return false;
    }

    public bool RemoveAllIncoming() {
      var didRemove = false;
      while (this.incomingNodes_.Count > 0) {
        didRemove |= this.RemoveIncoming(this.incomingNodes_.First);
      }

      return didRemove;
    }

    public bool RemoveAllOutgoing() {
      var didRemove = false;
      while (this.outgoingNodes_.Count > 0) {
        didRemove |= this.RemoveOutgoing(this.outgoingNodes_.First);
      }

      return didRemove;
    }
  }
}