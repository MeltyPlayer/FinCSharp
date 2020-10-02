using System.Collections.Generic;

namespace fin.data.collections.dictionary {
  public interface IMultiDictionary<TKey, TValue> {
    void Clear();

    IEnumerable<TKey> Keys { get; }

    IFinCollection<TValue> Get(TKey key);
    IFinCollection<TValue>? TryGet(TKey key);

    bool Add(TKey key, TValue value);

    bool Remove(TKey key, TValue value);

    void RemoveAll(TKey key);
  }
}