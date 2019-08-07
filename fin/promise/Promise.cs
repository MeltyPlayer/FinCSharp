using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fin.promise {
  public class Promise {
    public static Promise<T_RESULT> Resolve<T_RESULT>(T_RESULT value) {
      return new Promise<T_RESULT>((resolveHandler) => resolveHandler(value));
    }
  }

  public class Promise<T> : IAsyncResult {
    /*public static async IList<Promise> All(ICollection<Promise> promises) {
      const IList list = new List();
      await Task.WhenAll(promises);
    }*/

    private enum ConclusionType {
      UNDETERMINED,
      RESOLVED,
      REJECTED,
    }

    public delegate void Handler(T value);

    private T value_;
    private ConclusionType conclusion_;

    private List<Action<T>> thenResolveHandlers_ = new List<Action<T>>();

    public bool IsCompleted => throw new NotImplementedException();

    public WaitHandle AsyncWaitHandle => throw new NotImplementedException();

    public object AsyncState => throw new NotImplementedException();

    public bool CompletedSynchronously => throw new NotImplementedException();

    public Promise(Action<Handler> handler) {
      handler(ResolveInternal_);
    }

    public Promise<T_RESULT> Then<T_RESULT>(Func<T, T_RESULT> resolveHandler) {
      switch (conclusion_) {
        case ConclusionType.UNDETERMINED:\
          Promise<T_RESULT> nextPromise = new Promise<T_RESULT>(() => {
          });
          thenResolveHandlers_.Add((value) => {
            nextPromise.ResolveInternal_(resolveHandler(value));
          });
          return nextPromise;

        case ConclusionType.RESOLVED:
          return Promise.Resolve(resolveHandler(value_));

        default:
          throw new Exception("unhandled path, rejected then()");
      }
    }

    private void ResolveInternal_(T value) {
      conclusion_ = ConclusionType.RESOLVED;
      value_ = value;

      foreach (Action<T> thenResolveHandler in thenResolveHandlers_) {
        thenResolveHandler(value);
      }
      thenResolveHandlers_.Clear();
    }
  }
}
