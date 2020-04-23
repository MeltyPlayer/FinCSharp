using System.Collections.Generic;

namespace fin.data.collections.set {
  /// <summary>
  ///   An immutable view into a Fin set that cannot be cast as writable.
  /// </summary>
  public class ImmutablyWrappedFinSet<T> : IReadableFinSet<T> {
    private readonly IReadableFinSet<T> impl_;

    public ImmutablyWrappedFinSet(IReadableFinSet<T> impl) {
      this.impl_ = impl;
    }

    public int Count => this.impl_.Count;
    public IEnumerator<T> GetEnumerator() => this.impl_.GetEnumerator();
    public bool Contains(T value) => this.impl_.Contains(value);
  }
}