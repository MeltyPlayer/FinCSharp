using System.Collections.Concurrent;
using System.Collections.Generic;

using fin.data.collections.set;

// TODO: Add tests.
namespace fin.data.collections.dictionary {

  public class MultiDictionary<TKey, TValue> : IMultiDictionary<TKey, TValue> {
    private ConcurrentDictionary<TKey, OrderedSet<TValue>> impl_ = new ConcurrentDictionary<TKey, OrderedSet<TValue>>();

    public void Clear() {
      foreach (var values in this.impl_.Values) {
        values.Clear();
      }
      this.impl_.Clear();
    }

    public IEnumerable<TKey> Keys => this.impl_.Keys;

    public IEnumerable<TValue> Get(TKey key) => this.impl_[key];

    public bool Add(TKey key, TValue value) => this.impl_.GetOrAdd(key, key => new OrderedSet<TValue>()).Add(value);

    public bool Remove(TKey key, TValue value) {
      if (this.impl_.TryGetValue(key, out OrderedSet<TValue>? values)) {
        if (values.Remove(value)) {
          if (values.Count == 0) {
            this.impl_.TryRemove(key, out OrderedSet<TValue>? _);
          }
          return true;
        }
      }
      return false;
    }

    public void RemoveAll(TKey key) {
      if (this.impl_.TryRemove(key, out OrderedSet<TValue>? values)) {
        values.Clear();
      }
    }
  }
}