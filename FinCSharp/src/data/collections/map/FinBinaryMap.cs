﻿using System;
using System.Collections.Generic;

namespace fin.data.collections.map {
  public sealed class FinBinaryMap<TKey, TValue> : IFinMap<TKey, TValue> {
    private readonly IDictionary<TKey, TValue> impl_;

    public FinBinaryMap(IComparer<TKey> comparer) {
      this.impl_ = new SortedDictionary<TKey, TValue>(comparer);
    }

    public int Count => this.impl_.Count;

    public IEnumerator<(TKey, TValue)> GetEnumerator() =>
        new ConvertedEnumerator<KeyValuePair<TKey, TValue>, (TKey, TValue)>(
            this.impl_.GetEnumerator(),
            kvp => (kvp.Key, kvp.Value));

    public IEnumerable<TKey> Keys => this.impl_.Keys;
    public IEnumerable<TValue> Values => this.impl_.Values;
    public bool ContainsKey(TKey key) => this.impl_.ContainsKey(key);

    public bool TryGet(TKey key, out TValue value) =>
        this.impl_.TryGetValue(key, out value);


    public bool Clear() {
      if (this.Count == 0) {
        return false;
      }

      this.impl_.Clear();
      return true;
    }

    public bool TryAdd(TKey key, TValue value) =>
        this.impl_.TryAdd(key, value);

    public bool TryRemove(TKey key, out TValue value) =>
        this.impl_.Remove(key, out value);


    public TValue this[TKey key] {
      get => this.impl_[key];
      set => this.impl_[key] = value;
    }

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory) {
      if (this.impl_.TryGetValue(key, out var value)) {
        return value;
      }

      value = valueFactory(key);
      this.impl_.Add(key, value);
      return value;
    }
  }
}