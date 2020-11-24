using System;

using fin.assert;

namespace fin.math.number {
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

  public abstract class BRangedInt : BRangedNumber<int>, IRangedInt {
    protected BRangedInt(int min, int initialValue, int max) : base(
        min,
        initialValue,
        max) {}
  }

  public sealed class ClampedRangedInt : BRangedInt {
    private int impl_;

    public ClampedRangedInt(int min, int initialValue, int max) : base(
        min,
        initialValue,
        max) {
      this.Set(initialValue);
    }

    protected override int Get() => this.impl_;

    protected override void Set(int value) =>
        this.impl_ = IntMath.Clamp(this.Min, value, this.Max);
  }

  public sealed class CircularRangedInt : BRangedInt {
    private int impl_;

    public CircularRangedInt(int min, int initialValue, int max) : base(
        min,
        initialValue,
        max) {
      this.Set(initialValue);
    }

    protected override int Get() => this.impl_;

    protected override void Set(int value) =>
        this.impl_ = IntMath.Wrap(this.Min, value, this.Max);
  }


  public abstract class BRangedFloat : BRangedNumber<float>, IRangedFloat {
    protected BRangedFloat(float min, float initialValue, float max) : base(
        min,
        initialValue,
        max) {}
  }

  public sealed class ClampedRangedFloat : BRangedFloat {
    private float impl_;

    public ClampedRangedFloat(float min, float initialValue, float max) : base(
        min,
        initialValue,
        max) {
      this.Set(initialValue);
    }

    protected override float Get() => this.impl_;

    protected override void Set(float value) =>
        this.impl_ = FloatMath.Clamp(this.Min, value, this.Max);
  }

  public sealed class CircularRangedFloat : BRangedFloat {
    private float impl_;

    public CircularRangedFloat(float min, float initialValue, float max) : base(
        min,
        initialValue,
        max) {
      this.Set(initialValue);
    }

    protected override float Get() => this.impl_;

    protected override void Set(float value) =>
        this.impl_ = FloatMath.Wrap(this.Min, value, this.Max);
  }
}