﻿using System;

namespace fin.math.geometry {
  public interface IDimensions<TNumber> where TNumber : IComparable {
    TNumber Width { get; }
    TNumber Height { get; }
  }

  public class MutableDimensions<TNumber> : IDimensions<TNumber>
      where TNumber : IComparable {
    public TNumber Width { get; set; }
    public TNumber Height { get; set; }

    public MutableDimensions() {}

    public MutableDimensions(TNumber width, TNumber height) {
      this.Width = width;
      this.Height = height;
    }
  }

  public interface IBoundingBox<TNumber> where TNumber : IComparable {
    IVector2<TNumber> TopLeft { get; }
    IDimensions<TNumber> Dimensions { get; }

    // TODO: How slow is this dynamic stuff...?
    // TODO: Can we figure this out earlier to cache it?
    TNumber LeftX => FinMath.Min(this.TopLeft.X,
                              this.TopLeft.X +
                              (dynamic) this.Dimensions.Width);

    TNumber RightX => FinMath.Max(this.TopLeft.X,
                               this.TopLeft.X +
                               (dynamic) this.Dimensions.Width);

    TNumber CenterX => FinMath.Mean(this.LeftX, this.RightX);

    TNumber TopY => FinMath.Min(this.TopLeft.Y,
                             this.TopLeft.Y +
                             (dynamic) this.Dimensions.Height);

    TNumber BottomY => FinMath.Max(this.TopLeft.Y,
                                this.TopLeft.Y +
                                (dynamic) this.Dimensions.Height);

    TNumber CenterY => FinMath.Mean(this.TopY, this.BottomY);

    // TODO: Should we allow negative widths/heights? If we don't the above
    // stuff gets way easier.
    TNumber Width => FinMath.Abs(this.Dimensions.Width);
    TNumber Height => FinMath.Abs(this.Dimensions.Height);
  }

  public class AggregationBoundingBox<TNumber> : IBoundingBox<TNumber>
      where TNumber : IComparable {
    public IVector2<TNumber> TopLeft { get; }
    public IDimensions<TNumber> Dimensions { get; }

    public AggregationBoundingBox(
        IVector2<TNumber> topLeft,
        IDimensions<TNumber> dimensions) {
      this.TopLeft = topLeft;
      this.Dimensions = dimensions;
    }
  }

  public class MutableBoundingBox<TNumber> : IBoundingBox<TNumber>
      where TNumber : IComparable {
    IVector2<TNumber> IBoundingBox<TNumber>.TopLeft => this.TopLeft;
    public MutableVector2<TNumber> TopLeft { get; }

    IDimensions<TNumber> IBoundingBox<TNumber>.Dimensions => this.Dimensions;
    public MutableDimensions<TNumber> Dimensions { get; }

    public MutableBoundingBox() {
      this.TopLeft = new MutableVector2<TNumber>();
      this.Dimensions = new MutableDimensions<TNumber>();
    }

    public MutableBoundingBox(
        TNumber x,
        TNumber y,
        TNumber width,
        TNumber height) {
      this.TopLeft = new MutableVector2<TNumber>(x, y);
      this.Dimensions = new MutableDimensions<TNumber>(width, height);
    }
  }
}