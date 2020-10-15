using System;
using System.Collections.Generic;

namespace fin.generic {
  public class ConvergingTypedHandlerMap<TResult> {
    public delegate TResult Handler<in T>(T value);

    private readonly Dictionary<Type, object> handlers_ =
      new Dictionary<Type, object>();

    public void DefineHandler<T>(Handler<T> handler) {
      this.handlers_[typeof(T)] = handler;
    }

    public TResult Call<T>(T value) {
      // TODO: Go up the type tree if NPE.
      Type t = value?.GetType() ?? typeof(T);
      var untypedHandler = this.handlers_[t] as Handler<T>;
      return untypedHandler!(value);
    }
  }
}