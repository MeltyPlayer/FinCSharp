using System;
using System.Collections.Generic;
using System.Text;

namespace fin.data.collections.map {
  public interface IReadableFinMap<TKey, TValue> :
      IReadableFinCollection<(TKey, TValue)> {
    IEnumerable<TKey> Keys { get; }
    IEnumerable<TValue> Values { get; }
    bool ContainsKey(TKey key);
    TValue this[TKey key] { get; }
    bool TryGet(TKey key, out TValue value);

    TValue GetOrDefault(TKey key, TValue defaultValue) =>
        this.TryGet(key, out var value) ? value : defaultValue;
  }

  public interface IWritableFinMap<TKey, TValue> :
      IWritableFinCollection<(TKey, TValue)> {
    TValue this[TKey key] { set; }
    bool TryAdd(TKey key, TValue value);
    bool TryRemove(TKey key, out TValue value);
    bool TryRemove(TKey key) => this.TryRemove(key, out _);
  }

  public interface IFinMap<TKey, TValue> : IReadableFinMap<TKey, TValue>,
                                           IWritableFinMap<TKey, TValue> {
    new TValue this[TKey key] { get; set; }
    TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);
  }
}