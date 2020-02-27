using System.Collections;
using System.Collections.Generic;

// TODO: Add tests.
namespace fin.data.collections.set {
  // Based on https://stackoverflow.com/questions/1552225/hashset-that-preserves-ordering/17853085#17853085.
  public class OrderedSet<T> : ICollection<T> {
    private readonly IDictionary<T, LinkedListNode<T>> dictionary_;
    private readonly LinkedList<T> linkedList_;

    public OrderedSet() : this(EqualityComparer<T>.Default) { }

    public OrderedSet(IEqualityComparer<T> comparer) {
      this.dictionary_ = new Dictionary<T, LinkedListNode<T>>(comparer);
      this.linkedList_ = new LinkedList<T>();
    }

    public int Count => this.dictionary_.Count;

    public virtual bool IsReadOnly => this.dictionary_.IsReadOnly;

    // TODO: May be null.
    public T First => this.linkedList_.First!.Value;

    public T Last => this.linkedList_.Last!.Value;

    void ICollection<T>.Add(T item) => this.Add(item);

    public bool Add(T item) {
      if (this.dictionary_.ContainsKey(item)) {
        return false;
      }

      LinkedListNode<T> node = this.linkedList_.AddLast(item);
      this.dictionary_.Add(item, node);
      return true;
    }

    public void Clear() {
      this.dictionary_.Clear();
      this.linkedList_.Clear();
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

    public IEnumerator<T> GetEnumerator() => this.linkedList_.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public bool Contains(T item) => this.dictionary_.ContainsKey(item);

    public void CopyTo(T[] array, int arrayIndex) =>
      this.linkedList_.CopyTo(array, arrayIndex);
  }
}