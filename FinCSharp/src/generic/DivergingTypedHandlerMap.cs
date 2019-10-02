using System;
using System.Collections.Generic;

namespace fin.generic {

  public class DivergingTypedHandlerMap<T> {

    public delegate TResult Handler<out TResult>(T value);

    private readonly Dictionary<Type, Handler<object>> handlers_ =
      new Dictionary<Type, Handler<object>>();

    public void DefineHandler<TResult>(Handler<TResult> handler) {
      var untypedHandler = handler as Handler<object>;
      this.handlers_[typeof(TResult)] = untypedHandler!;
    }

    public TResult Call<TResult>(T value) {
      var untypedHandler = this.handlers_[typeof(TResult)] as Handler<TResult>;
      return untypedHandler!(value);
    }
  }
}