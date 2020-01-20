using System.Collections.Generic;
using System.Linq;
using fin.assert;

namespace fin.data.tree {

  public interface INode<T> {
    T Value { set; get; }
    IEnumerable<INode<T>> ToNodes { get; }
    IEnumerable<INode<T>> FromNodes { get; }

    bool AddTo(INode<T> other);

    bool RemoveTo(INode<T> other);

    bool AddFrom(INode<T> other);

    bool RemoveFrom(INode<T> other);
  }

  public class Node<T> : INode<T> {
    private T value_;
    private readonly ISet<INode<T>> fromNodes_ = new HashSet<INode<T>>();
    private readonly ISet<INode<T>> toNodes_ = new HashSet<INode<T>>();

    public Node(T value) {
      this.value_ = value;
    }

    public T Value { get => this.value_; set => this.value_ = value; }

    public IEnumerable<INode<T>> FromNodes => this.fromNodes_;

    public IEnumerable<INode<T>> ToNodes => this.toNodes_;

    public bool AddFrom(INode<T> other) {
      if (this.fromNodes_.Add(other)) {
        other.AddTo(this);
        return true;
      }
      return false;
    }

    public bool RemoveFrom(INode<T> other) {
      if (this.fromNodes_.Remove(other)) {
        other.RemoveTo(this);
        return true;
      }
      return false;
    }

    public bool AddTo(INode<T> other) {
      if (this.toNodes_.Add(other)) {
        other.AddFrom(this);
        return true;
      }
      return false;
    }

    public bool RemoveTo(INode<T> other) {
      if (this.toNodes_.Remove(other)) {
        other.RemoveFrom(this);
        return true;
      }
      return false;
    }
  }
}