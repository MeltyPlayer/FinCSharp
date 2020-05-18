using System.Collections.Generic;

namespace fin.data.collections.list {
  public class FinVectorList<T> : IFinList<T> {
    private readonly List<T> impl_ = new List<T>();

    public int Count => this.impl_.Count;

    public IEnumerator<T> GetEnumerator() => this.impl_.GetEnumerator();

    public bool Clear() {
      if (this.Count == 0) {
        return false;
      }

      this.impl_.Clear();
      return true;
    }

    public T this[int index] {
      get => this.impl_[index];
      set => this.impl_[index] = value;
    }
  }
}