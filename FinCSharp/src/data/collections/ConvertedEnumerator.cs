using System;
using System.Collections;
using System.Collections.Generic;

namespace fin.data.collections {
  public sealed class ConvertedEnumerator<TOutput> : IEnumerator<TOutput> {
    private readonly IEnumerator impl_;
    private readonly Func<object, TOutput> conversionFunction_;

    public ConvertedEnumerator(
        IEnumerator impl,
        Func<object, TOutput> conversionFunction) {
      this.impl_ = impl;
      this.conversionFunction_ = conversionFunction;
    }

    public TOutput Current =>
        this.conversionFunction_(this.impl_.Current!);

    object? IEnumerator.Current => this.Current;
    public void Dispose() {}
    public bool MoveNext() => this.impl_.MoveNext();
    public void Reset() => this.impl_.Reset();
  }

  public sealed class
      ConvertedEnumerator<TInput, TOutput> : IEnumerator<TOutput> {
    private readonly IEnumerator<TInput> impl_;
    private readonly Func<TInput, TOutput> conversionFunction_;

    public ConvertedEnumerator(
        IEnumerator<TInput> impl,
        Func<TInput, TOutput> conversionFunction) {
      this.impl_ = impl;
      this.conversionFunction_ = conversionFunction;
    }

    public TOutput Current =>
        this.conversionFunction_(this.impl_.Current);

    object? IEnumerator.Current => this.Current;
    public void Dispose() => this.impl_.Dispose();
    public bool MoveNext() => this.impl_.MoveNext();
    public void Reset() => this.impl_.Reset();
  }
}