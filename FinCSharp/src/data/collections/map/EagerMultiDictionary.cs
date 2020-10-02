using System.Collections.Generic;

using fin.data.collections.map;
using fin.data.collections.set;

// TODO: Add tests.
namespace fin.data.collections.dictionary {
  public class
      EagerMultiDictionary<TKey, TValue> : IMultiDictionary<TKey, TValue> {
    private readonly IFinMap<TKey, IFinSet<TValue>> impl_ =
        new FinHashMap<TKey, IFinSet<TValue>>();

    public void Clear() {
      foreach (var values in this.impl_.Values) {
        values.Clear();
      }
      this.impl_.Clear();
    }

    public IEnumerable<TKey> Keys => this.impl_.Keys;

    public IFinCollection<TValue> Get(TKey key) => this.GetOrAdd_(key);

    private IFinSet<TValue> GetOrAdd_(TKey key)
      => this.impl_.GetOrAdd(key, _ => new FinHashSet<TValue>());

    public IFinCollection<TValue>? TryGet(TKey key)
      => this.impl_.TryGet(key, out var values) ? values : null;

    public bool Add(TKey key, TValue value) => this.GetOrAdd_(key).Add(value);

    public bool Remove(TKey key, TValue value)
      => this.impl_.TryGet(key, out var values) && values.Remove(value);

    public void RemoveAll(TKey key) {
      if (this.impl_.TryRemove(key, out var values)) {
        values.Clear();
      }
    }
  }
}