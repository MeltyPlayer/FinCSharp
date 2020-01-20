using System.Collections;
using System.Collections.Generic;

namespace fin.data.collections.set {

  public delegate void OnAddEventHandler<T>(params T[] items);

  public delegate void OnRemoveEventHandler<T>(params T[] items);

  public interface IImmutableFinSet<T> : IEnumerable {

    event OnAddEventHandler<T> OnAddEvent;

    event OnRemoveEventHandler<T> OnRemoveEvent;

    int Count { get; }

    bool Contains(T item);
  }

  public interface IFinSet<T> : IImmutableFinSet<T> {

    void Clear();

    bool Add(T item);

    bool Remove(T item);
  }

  public class HashFinSet<T> : IFinSet<T> {
    private readonly ISet<T> impl_ = new HashSet<T>();

    public event OnAddEventHandler<T> OnAddEvent = delegate { };

    public event OnRemoveEventHandler<T> OnRemoveEvent = delegate { };

    public HashFinSet() {
    }

    public int Count => this.impl_.Count;

    public void Clear() {
      var count = this.Count;
      if (count > 0) {
        T[] temp = new T[count];
        this.impl_.CopyTo(temp, 0);
        this.impl_.Clear();
        this.OnRemoveEvent(temp);
      }
    }

    public bool Contains(T item) => this.impl_.Contains(item);

    public bool Add(T item) {
      if (this.impl_.Add(item)) {
        this.OnAddEvent(item);
        return true;
      }
      return false;
    }

    public bool Remove(T item) {
      if (this.impl_.Remove(item)) {
        this.OnRemoveEvent(item);
        return true;
      }
      return false;
    }

    public IEnumerator<T> GetEnumerator() => this.impl_.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.impl_.GetEnumerator();
  }
}