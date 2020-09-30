using System.Collections.Generic;

namespace fin.data.collections.list {
  /// <summary>
  ///   An immutable view into a Fin list that cannot be cast as writable.
  /// </summary>
  public class ImmutablyWrappedFinList<T> : IReadableFinList<T> {
    private readonly IReadableFinList<T> impl_;

    public ImmutablyWrappedFinList(IReadableFinList<T> impl) {
      this.impl_ = impl;
    }

    public int Count => this.impl_.Count;
    public IEnumerator<T> GetEnumerator() => this.impl_.GetEnumerator();

    public T this[int index] => this.impl_[index];
  }
}