using System.Collections;
using System.Collections.Generic;

namespace fin.data.collections {
  public interface IFinCollection<T> : IReadableFinCollection<T>,
                                       IWritableFinCollection<T> {}

  public interface IReadableFinCollection<out T> : IEnumerable<T> {
    int Count { get; }
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
  }

  public interface IWritableFinCollection<T> {
    bool Clear();
  }
}