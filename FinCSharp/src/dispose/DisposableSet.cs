using System.Collections.Generic;
using fin.data.collections.set;

namespace fin.dispose {
  /*public class DisposableSet<TDisposable> : IFinSet<TDisposable>
    where TDisposable : UnsafeDisposable {
    private readonly ISet<TDisposable> set_;

    public bool Add(TDisposable instance) {
      return this.set_.Add(instance);
    }

    public bool Remove(TDisposable instance) {
      return this.set_.Remove(instance);
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public IEnumerator<TDisposable> GetEnumerator() {
      throw new System.NotImplementedException();
    }
  }

  public class DisposableEnumerator<TDisposable> : IEnumerator<TDisposable>
    where TDisposable : UnsafeDisposable {
    private readonly IEnumerator<TDisposable> enumerator_;

    public DisposableEnumerator(IEnumerator<TDisposable> enumerator) {
      this.enumerator_ = enumerator;
    }

    public void Dispose() {
      this.enumerator_.Dispose();
    }

    public bool MoveNext() {
      var successful = this.enumerator_.MoveNext();
      // TODO: Move null check to a "NonnullSet".
      while (successful &&
             (this.enumerator_.Current == null ||
              this.enumerator_.Current.IsDisposed())) {
        successful |= this.enumerator_.MoveNext();
      }
      return successful;
    }

    public void Reset () {
      this.enumerator_.Reset();
    }

    object System.Collections.IEnumerator.Current => this.Current;
    public TDisposable Current => this.enumerator_.Current;
  }*/
}