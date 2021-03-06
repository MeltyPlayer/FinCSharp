﻿using System;

namespace fin.math.geometry {
  public interface IVector2<TNumber> where TNumber : IComparable {
    TNumber X { get; }
    TNumber Y { get; }
  }

  public class MutableVector2<TNumber> : IVector2<TNumber>
    where TNumber : IComparable {
    public TNumber X { get; set; } = default;
    public TNumber Y { get; set; } = default;

    public MutableVector2() { }

    public MutableVector2(TNumber x, TNumber y)
      => (this.X, this.Y) = (x, y);
  }

  public class ImmutableVector2<TNumber> : IVector2<TNumber>
    where TNumber : IComparable {
    public TNumber X { get; } = default;
    public TNumber Y { get; } = default;

    public ImmutableVector2() { }

    public ImmutableVector2(TNumber x, TNumber y)
      => (this.X, this.Y) = (x, y);
  }
}