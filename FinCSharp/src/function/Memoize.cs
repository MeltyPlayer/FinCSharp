using System;
using System.Collections.Concurrent;

namespace fin.function {

  public class MemoizedFunc<T, TResult> where T : notnull {
    public ConcurrentDictionary<T, TResult> Cache { get; } = new ConcurrentDictionary<T, TResult>();

    private readonly Func<T, TResult> handler_;

    public MemoizedFunc(Func<T, TResult> handler) {
      this.handler_ = handler;
    }

    public TResult Invoke(T a) => this.Cache.GetOrAdd(a, this.handler_);
  }

  public class MemoizedFunc<T1, T2, TResult> where T1 : notnull where T2 : notnull {
    public ConcurrentDictionary<Tuple<T1, T2>, TResult> Cache { get; } = new ConcurrentDictionary<Tuple<T1, T2>, TResult>();

    private readonly Func<T1, T2, TResult> handler_;

    public MemoizedFunc(Func<T1, T2, TResult> handler) {
      this.handler_ = handler;
    }

    private TResult InvokeWithTuple_(Tuple<T1, T2> a1AndA2) => this.handler_(a1AndA2.Item1, a1AndA2.Item2);

    public TResult Invoke(T1 a1, T2 a2) => this.Cache.GetOrAdd(Tuple.Create(a1, a2), this.InvokeWithTuple_);
  }

  public static class Memoization {

    public static MemoizedFunc<T, TResult> MemoizeDebug<T, TResult>(this Func<T, TResult> f) where T : notnull
      => new MemoizedFunc<T, TResult>(f);

    public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> f) where T : notnull
      => new MemoizedFunc<T, TResult>(f).Invoke;

    public static MemoizedFunc<T1, T2, TResult> MemoizeDebug<T1, T2, TResult>(this Func<T1, T2, TResult> f) where T1 : notnull where T2 : notnull
      => new MemoizedFunc<T1, T2, TResult>(f);

    public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(this Func<T1, T2, TResult> f) where T1 : notnull where T2 : notnull
      => new MemoizedFunc<T1, T2, TResult>(f).Invoke;
  }
}