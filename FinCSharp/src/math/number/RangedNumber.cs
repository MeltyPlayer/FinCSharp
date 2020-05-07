using System;

using fin.assert;

namespace fin.math.number {
  public interface IRangedNumber<TNumber> : INumber<TNumber>
      where TNumber : IComparable {
    TNumber Min { get; }
    TNumber Max { get; }
  }

  public abstract class BRangedNumber<TNumber> : IRangedNumber<TNumber>
      where TNumber : IComparable {
    public TNumber Min { get; }

    public TNumber Value {
      get => this.Get();
      set => this.Set(value);
    }

    public TNumber Max { get; }

    public BRangedNumber(TNumber min, TNumber initialValue, TNumber max) {
      Asserts.True(min.CompareTo(max) < 0, "Min must be less than max.");
      Asserts.True(min.CompareTo(initialValue) <= 0,
                   "Initial value must >= min.");
      Asserts.True(initialValue.CompareTo(max) <= 0,
                   "Initial value must <= max.");

      this.Min = min;
      this.Max = max;
    }

    protected abstract TNumber Get();
    protected abstract void Set(TNumber value);
  }

  public sealed class ClampedRangedNumber<TNumber> : BRangedNumber<TNumber>
      where TNumber : IComparable {
    private TNumber impl_ = default;

    public ClampedRangedNumber(
        TNumber min,
        TNumber initialValue,
        TNumber max) : base(
        min,
        initialValue,
        max) {
      this.Set(initialValue);
    }

    protected override TNumber Get() => this.impl_;

    protected override void Set(TNumber value) =>
        this.impl_ = Math.Clamp(this.Min, value, this.Max);
  }

  public sealed class CircularRangedNumber<TNumber> : BRangedNumber<TNumber>
      where TNumber : IComparable {
    private TNumber impl_ = default;

    public CircularRangedNumber(
        TNumber min,
        TNumber initialValue,
        TNumber max) : base(
        min,
        initialValue,
        max) {
      this.Set(initialValue);
    }

    protected override TNumber Get() => this.impl_;

    protected override void Set(TNumber value) =>
        this.impl_ = Math.Wrap(this.Min, value, this.Max);
  }
}