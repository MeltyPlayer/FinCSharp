using System.Collections;
using System.Collections.Generic;

namespace fin.data.collections.list {
  public class FinArrayList<T> : IFinList<T> {
    private readonly ArrayList impl_;

    public FinArrayList() {
      this.impl_ = new ArrayList();
    }

    public FinArrayList(int size, T defaultValue = default) {
      this.impl_ = new ArrayList();
      for (var i = 0; i < size; ++i) {
        this.impl_.Add(defaultValue);
      }
    }

    public int Count => this.impl_.Count;

    public IEnumerator<T> GetEnumerator() =>
        new ConvertedEnumerator<T>(this.impl_.GetEnumerator(),
                                   obj => (T) obj);

    public bool Clear() {
      if (this.Count == 0) {
        return false;
      }

      this.impl_.Clear();
      return true;
    }

    public T this[int index] {
      get => (T) this.impl_[index]!;
      set => this.impl_[index] = value;
    }
  }
}