using System.Collections;
using System.Collections.Generic;

namespace fin.data.enumerator {
  public sealed class AggregateEnumerator<T> : IEnumerator<T> {
    private readonly IEnumerable<T>[] impls_;

    private int implIndex_;
    private IEnumerator<T>? currentEnumerator_;

    public AggregateEnumerator(params IEnumerable<T>[] impls) {
      this.impls_ = impls;
      this.Reset();
    }

    public void Dispose() => this.currentEnumerator_?.Dispose();

    public T Current {
      get {
        if (this.currentEnumerator_ != null) {
          return this.currentEnumerator_.Current;
        }

        return default!;
      }
    }

    object? IEnumerator.Current => this.Current;

    public bool MoveNext() {
      if (this.currentEnumerator_ != null &&
          this.currentEnumerator_.MoveNext()) {
        return true;
      }

      // TODO: Refactor this to not use a while loop.
      while (this.MoveTo_(++this.implIndex_)) {
        if (this.currentEnumerator_!.MoveNext()) {
          return true;
        }
      }

      return false;
    }

    public void Reset() => this.MoveTo_(-1);

    private bool MoveTo_(int newIndex) {
      this.currentEnumerator_?.Dispose();

      this.implIndex_ = newIndex;
      if (this.implIndex_ >= 0 && this.implIndex_ < this.impls_.Length) {
        var currentImpl = this.impls_[this.implIndex_];
        this.currentEnumerator_ = currentImpl.GetEnumerator();
        return true;
      }

      return false;
    }
  }
}