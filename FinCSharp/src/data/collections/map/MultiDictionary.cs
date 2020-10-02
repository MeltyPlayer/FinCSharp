using System.Collections.Generic;

using fin.data.collections.map;
using fin.data.collections.set;

// TODO: Add tests.
namespace fin.data.collections.dictionary {
  public class MultiDictionary<TKey, TValue> : IMultiDictionary<TKey, TValue> {
    private readonly IFinMap<TKey, IFinSet<TValue>> impl_ =
        new FinHashMap<TKey, IFinSet<TValue>>();

    public void Clear() {
      foreach (var values in this.impl_.Values) {
        values.Clear();
      }

      this.impl_.Clear();
    }

    public IEnumerable<TKey> Keys => this.impl_.Keys;

    public IFinCollection<TValue> Get(TKey key) => this.impl_[key];
    public IFinCollection<TValue>? TryGet(TKey key) {
      try {
        return this.Get(key);
      }
      catch {
        return null;
      }
    }

    public bool Add(TKey key, TValue value) => this.impl_
                                                   .GetOrAdd(key, _ => new FinHashSet<TValue>()).Add(value);

    public bool Remove(TKey key, TValue value) {
      if (!this.impl_.TryGet(key, out var values) || !values.Remove(value)) {
        return false;
      }

      if (values.Count == 0) {
        this.impl_.TryRemove(key);
      }

      return true;
    }

    public void RemoveAll(TKey key) {
      if (this.impl_.TryRemove(key, out var values)) {
        values.Clear();
      }
    }
  }
}