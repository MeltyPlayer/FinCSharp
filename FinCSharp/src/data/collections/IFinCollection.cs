using System.Collections.Generic;

namespace fin.data.collections {
  public interface IFinCollection<out T> : IEnumerable<T> {
    int Count { get; }
  }
}
