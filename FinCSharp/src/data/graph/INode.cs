using System.Collections.Generic;

namespace fin.data.graph {
  /// <summary>
  ///   An interface for a graph node.
  /// </summary>
  public interface INode<T> {
    T Value { set; get; }
    IEnumerable<INode<T>> OutgoingNodes { get; }
    IEnumerable<INode<T>> IncomingNodes { get; }

    /// <summary>
    ///   Adds an incoming edge from another node.
    /// </summary>
    /// <returns>
    ///   Whether the edge was newly added.
    /// </returns>
    bool AddIncoming(INode<T> other);

    /// <summary>
    ///   Adds an outgoing edge to another node.
    /// </summary>
    /// <returns>
    ///   Whether the edge was newly added.
    /// </returns>
    bool AddOutgoing(INode<T> other);

    /// <summary>
    ///   Removes an incoming edge from another node.
    /// </summary>
    /// <returns>
    ///   Whether the edge was newly removed.
    /// </returns>
    bool RemoveIncoming(INode<T> other);

    /// <summary>
    ///   Removes an outgoing edge to another node.
    /// </summary>
    /// <returns>
    ///   Whether the edge was newly removed.
    /// </returns>
    bool RemoveOutgoing(INode<T> other);

    /// <summary>
    ///   Removes all incoming edges.
    /// </summary>
    /// <returns>
    ///   Whether any edges were removed.
    /// </returns>
    bool RemoveAllIncoming();

    /// <summary>
    ///   Removes all outgoing edges.
    /// </summary>
    /// <returns>
    ///   Whether any edges were removed.
    /// </returns>
    bool RemoveAllOutgoing();
  }
}