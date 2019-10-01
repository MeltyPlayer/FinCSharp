using System;
using System.Collections.Concurrent;

namespace fin.function {

  public static class Memoization {

    public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> f) where T : notnull {
      var cache = new ConcurrentDictionary<T, TResult>();
      return a => cache.GetOrAdd(a, f);
    }

    public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(this Func<T1, T2, TResult> f) where T1 : notnull where T2 : notnull {
      var cache = new ConcurrentDictionary<Tuple<T1, T2>, TResult>();
      return (a1, a2) => cache.GetOrAdd(Tuple.Create(a1, a2), (a1AndA2) => f(a1AndA2.Item1, a1AndA2.Item2));
    }
  }
}