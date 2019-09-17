using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.generic {
  public class DivergingTypedHandlerMap<T> {
    public delegate TResult Handler<out TResult>(T value);

    private readonly Dictionary<Type, Handler<object>> handlers_ =
      new Dictionary<Type, Handler<object>>();

    public void DefineHandler<TResult>(Handler<TResult> handler) {
      this.handlers_[typeof(TResult)] = handler as Handler<object>;
    }

    public TResult Call<TResult>(T value) {
      return (this.handlers_[typeof(TResult)] as Handler<TResult>)(value);
    }
  }
}