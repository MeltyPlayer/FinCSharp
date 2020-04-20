using System.Collections;
using System.Collections.Generic;

using fin.data.collections.list;

// TODO: Add tests.
namespace fin.data.collections.set {
  // Based on https://stackoverflow.com/questions/1552225/hashset-that-preserves-ordering/17853085#17853085.
  public class FinOrderedSet<T> : IFinSet<T> {
    private readonly IDictionary<T, LinkedListNode<T>> dictionary_;
    private readonly LinkedList<T> linkedList_;

    public FinOrderedSet() : this(EqualityComparer<T>.Default) {}

    public FinOrderedSet(IEqualityComparer<T> comparer) {
      this.dictionary_ = new Dictionary<T, LinkedListNode<T>>(comparer);
      this.linkedList_ = new LinkedList<T>();
    }

    public int Count => this.dictionary_.Count;
    public IEnumerator<T> GetEnumerator() => this.linkedList_.GetEnumerator();
    public bool Contains(T item) => this.dictionary_.ContainsKey(item);

    public bool Clear() {
      if (this.Count == 0) {
        return false;
      }

      this.dictionary_.Clear();
      this.linkedList_.Clear();
      return true;
    }

    public bool Add(T item) {
      if (this.dictionary_.ContainsKey(item)) {
        return false;
      }

      var node = this.linkedList_.AddLast(item);
      this.dictionary_.Add(item, node);
      return true;
    }

    public bool Remove(T item) {
      if (!this.dictionary_.ContainsKey(item)) {
        return false;
      }

      var node = this.dictionary_[item];
      this.dictionary_.Remove(item);
      this.linkedList_.Remove(node);
      return true;
    }
  }
}