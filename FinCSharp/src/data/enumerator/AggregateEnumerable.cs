using System.Collections;
using System.Collections.Generic;

namespace fin.data.enumerator {
  public sealed class AggregateEnumerable<T> : IEnumerable<T> {
    private readonly IEnumerable<T>[] impls_;

    public AggregateEnumerable(params IEnumerable<T>[] impls) {
      this.impls_ = impls;
    }

    public IEnumerator<T> GetEnumerator()
      => new AggregateEnumerator<T>(this.impls_);

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
  }
}