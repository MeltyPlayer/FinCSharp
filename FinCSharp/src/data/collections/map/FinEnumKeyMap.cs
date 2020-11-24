/*using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fin.data.collections.map {
  /// <summary>
  ///   A super-fast "map" from an enum key to a value that leverages the fact
  ///   that enums are ints under the hood.
  /// </summary>
  public class FinEnumKeyMap<TEnumKey, TValue> : IFinMap<TEnumKey, TValue> where TEnumKey : Enum {
    private readonly TEnumKey[] keys_;
    private readonly TValue[] values_;
    private readonly bool[] contains_;

    public FinEnumKeyMap() {
      this.keys_ = (TEnumKey[]) Enum.GetValues(typeof(TEnumKey));

      var count = this.keys_.Length;
      this.values_ = new TValue[count];
      this.contains_ = new bool[count];
    }

    public int Count => this.keys_.Length;

    public IEnumerator<(TEnumKey, TValue)> GetEnumerator() =>
        new ConvertedEnumerator<KeyValuePair<TEnumKey, TValue>, (TEnumKey, TValue)>(
            this.impl_.GetEnumerator(),
            kvp => (kvp.Key, kvp.Value));

    public IEnumerable<TEnumKey> Keys => this.keys_;
    public IEnumerable<TValue> Values => this.values_;
    public bool ContainsKey(TEnumKey key) => this.contains_[()];

    public bool TryGet(TEnumKey key, out TValue value) =>
        this.impl_.TryGetValue(key, out value);


    public bool Clear() {
      if (this.Count == 0) {
        return false;
      }

      this.impl_.Clear();
      return true;
    }

    public bool TryAdd(TEnumKey key, TValue value) =>
        this.impl_.TryAdd(key, value);

    public bool TryRemove(TEnumKey key, out TValue value) =>
        this.impl_.TryRemove(key, out value);


    public TValue this[TEnumKey key] {
      get => this.impl_[key];
      set => this.impl_[key] = value;
    }

    public TValue GetOrAdd(TEnumKey key, Func<TEnumKey, TValue> valueFactory) =>
        this.impl_.GetOrAdd(key, valueFactory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int EnumToIndex_(TEnumKey enumKey) => Convert.ToInt32(enumKey);
  }
}*/