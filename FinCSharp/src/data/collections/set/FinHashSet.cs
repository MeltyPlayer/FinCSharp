using System.Collections.Generic;

namespace fin.data.collections.set {
  public sealed class FinHashSet<T> : IFinSet<T> {
    private readonly ISet<T> impl_ = new HashSet<T>();

    public int Count => this.impl_.Count;
    public IEnumerator<T> GetEnumerator() => this.impl_.GetEnumerator();
    public bool Contains(T value) => this.impl_.Contains(value);

    public bool Clear() {
      if (this.Count == 0) {
        return false;
      }

      this.impl_.Clear();
      return true;
    }

    public bool Add(T value) => this.impl_.Add(value);
    public bool Remove(T value) => this.impl_.Remove(value);
  }
}