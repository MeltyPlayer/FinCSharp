using System.Collections.Generic;

namespace fin.data.graph {
  /// <summary>
  ///   A basic implementation of a graph node.
  /// </summary>
  public class Node<T> : INode<T> {
    private readonly ISet<INode<T>> incomingNodes_ = new HashSet<INode<T>>();
    private readonly ISet<INode<T>> outgoingNodes_ = new HashSet<INode<T>>();

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
      foreach (var other in this.incomingNodes_) {
        didRemove |= this.RemoveIncoming(other);
      }

      return didRemove;
    }

    public bool RemoveAllOutgoing() {
      var didRemove = false;
      foreach (var other in this.outgoingNodes_) {
        didRemove |= this.RemoveOutgoing(other);
      }

      return didRemove;
    }
  }
}