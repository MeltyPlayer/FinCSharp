using System;
using System.Collections.Generic;

using fin.data.collections.list;
using fin.math.number;

namespace fin.data.collections.buffer {
  public class FinCircularBuffer<T> : IFinBuffer<T> {
    private readonly IFinList<T> impl_;
    private readonly IRangedInt index_;

    public FinCircularBuffer(int size, Func<T> generator) {
      this.impl_ = new FinArrayList<T>(size);
      for (var i = 0; i < size; ++i) {
        this.impl_[i] = generator();
      }

      this.index_ = new CircularRangedInt(0, 0, size);
    }

    public int Count => this.impl_.Count;

    public IEnumerator<T> GetEnumerator() => this.impl_.GetEnumerator();

    public bool Clear() => this.impl_.Clear();

    public T Next => this.impl_[this.index_.Value++];
  }
}