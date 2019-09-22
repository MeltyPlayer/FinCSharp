using System;
using System.Collections.Generic;

namespace fin.generic {

  public class ConvergingTypedHandlerMap<TResult> {

    public delegate TResult Handler<in T>(T value);

    private readonly Dictionary<Type, Handler<object>> handlers_ = new Dictionary<Type, Handler<object>>();

    public void DefineHandler<T>(Handler<T> handler) {
      var untypedHandler = handler as Handler<object>;
      this.handlers_[typeof(T)] = untypedHandler!;
    }

    public TResult Call<T>(T value) {
      // TODO: Go up the type tree if NPE.
      Type t = value?.GetType() ?? typeof(T);
      var untypedHandler = this.handlers_[t] as Handler<T>;
      return untypedHandler!(value);
    }
  }
}