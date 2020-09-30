using System;

namespace fin.math.geometry {
  public interface IDimensions<TNumber> where TNumber : IComparable {
    TNumber Width { get; }
    TNumber Height { get; }
  }

  public class MutableDimensions<TNumber> : IDimensions<TNumber>
      where TNumber : IComparable {
    public TNumber Width { get; set; } = default;
    public TNumber Height { get; set; } = default;

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
    TNumber LeftX => Math.Min(this.TopLeft.X,
                              this.TopLeft.X +
                              (dynamic) this.Dimensions.Width);

    TNumber RightX => Math.Max(this.TopLeft.X,
                               this.TopLeft.X +
                               (dynamic) this.Dimensions.Width);

    TNumber CenterX => Math.Mean(this.LeftX, this.RightX);

    TNumber TopY => Math.Min(this.TopLeft.Y,
                             this.TopLeft.Y +
                             (dynamic) this.Dimensions.Height);

    TNumber BottomY => Math.Max(this.TopLeft.Y,
                                this.TopLeft.Y +
                                (dynamic) this.Dimensions.Height);

    TNumber CenterY => Math.Mean(this.TopY, this.BottomY);

    // TODO: Should we allow negative widths/heights? If we don't the above
    // stuff gets way easier.
    TNumber Width => Math.Abs(this.Dimensions.Width);
    TNumber Height => Math.Abs(this.Dimensions.Height);
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