using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.generic {
  public class ConvergingTypedHandlerMap<TResult> {
    public delegate TResult Handler<in T>(T value);

    private readonly Dictionary<Type, Handler<object>> handlers_ =
      new Dictionary<Type, Handler<object>>();

    public void DefineHandler<T>(Handler<T> handler) {
      this.handlers_[typeof(T)] = handler as Handler<object>;
    }

    public TResult Call<T>(T value) {
      // TODO: Go up the type tree if NPE.
      return (this.handlers_[value.GetType()] as Handler<T>)(value);
    }
  }
}