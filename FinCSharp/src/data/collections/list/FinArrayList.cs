using System.Collections;
using System.Collections.Generic;

namespace fin.data.collections.list {
  public class FinArrayList<T> : IFinList<T> {
    private readonly List<T> impl_;

    public FinArrayList() {
      this.impl_ = new List<T>();
    }

    public FinArrayList(int size, T defaultValue = default) {
      this.impl_ = new List<T>(size);
      for (var i = 0; i < size; ++i) {
        this.impl_.Add(defaultValue);
      }
    }

    public int Count => this.impl_.Count;

    public IEnumerator<T> GetEnumerator() => this.impl_.GetEnumerator();

    public bool Clear() {
      if (this.Count == 0) {
        return false;
      }

      this.impl_.Clear();
      return true;
    }

    /// <remarks>
    /// Time: O(1)
    /// </remarks>
    public T this[int index] {
      get => this.impl_[index]!;
      set => this.impl_[index] = value;
    }
  }
}