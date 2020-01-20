/*using System.Collections.Generic;
using fin.data.collections.set;

namespace fin.data.collections.dictionary {
  public delegate void OnAddEventHandler<TKey, TValue>(params KeyValuePair<TKey, TValue>[] items);

  public delegate void OnRemoveEventHandler<TKey, TValue>(params KeyValuePair<TKey, TValue>[] items);

  public interface IImmutableFinDictionary<TKey, TValue> {
    event OnAddEventHandler<TKey, TValue> OnAddEvent;

    event OnRemoveEventHandler<TKey, TValue> OnRemoveEvent;

    int Count { get; }

    IImmutableFinSet<TKey> Keys { get; }

    IImmutableFinSet<TValue> Values { get; }

    TValue this[TKey key] { get; }
  }

  public interface IFinDictionary<TKey, TValue> : IImmutableFinDictionary<TKey, TValue> {
    void Clear();

    bool Add(TKey key, TValue value);

    bool Remove(TKey key);

    new TValue this[TKey key] { set; get; }
  }

  public class FinDictionary<TKey, TValue> : IFinDictionary<TKey, TValue> {
    private readonly IDictionary<TKey, TValue> impl_ = new Dictionary<TKey, TValue>();

    public FinDictionary() {
    }

    public int Count => this.impl_.Count;

    public void Clear() {
      var count = this.Count;
      if (count > 0) {
        T[] temp = new T[count];
        this.impl_.Keys.CopyTo(temp, 0);
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
  }
}*/