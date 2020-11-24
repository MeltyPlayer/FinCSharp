using System;

namespace fin.math.number {
  public interface IDirection {
    float Fraction { get; set; }

    float Degrees { get; set; }
    float Radians { get; set; }

    float NormalX { get; }
    float NormalY { get; }
    (float, float) Normal { get; set; }
  }

  public class Direction : IDirection {
    private readonly IFraction impl_;

    public float Fraction {
      get => this.impl_.Value;
      set => this.Set_(value);
    }

    public float Degrees {
      get => this.Fraction * 360;
      set => this.Fraction = value / 360;
    }

    public float Radians {
      get => this.impl_.Value * FinMath.TAU;
      set => this.Fraction = value / FinMath.TAU;
    }

    private float? normalX_;
    private float? normalY_;

    public float NormalX
      => this.normalX_ ?? (this.normalX_ = (float) Math.Cos(this.Radians)) ?? 0;

    public float NormalY
      => this.normalY_ ?? (this.normalY_ = (float) Math.Sin(this.Radians)) ?? 0;

    public (float, float) Normal {
      get => (this.NormalX, this.NormalY);
      set {
        var (normalX, normalY) = value;
        this.Radians = (float) Math.Atan2(normalY, normalX);
      }
    }

    private Direction() {
      this.impl_ = new CircularFraction(0);
    }

    public static Direction FromDegrees(float degrees) =>
        new Direction() {
            Degrees = degrees,
        };

    public static Direction FromRadians(float radians) =>
        new Direction() {
            Radians = radians,
        };

    public static Direction FromNormal(float normalX, float normalY) =>
        new Direction() {
            Normal = (normalX, normalY),
        };

    private void Set_(float value) {
      var initialValue = this.impl_.Value;
      this.impl_.Value = value;
      var finalValue = this.impl_.Value;

      // TODO: Make sure this tolerance works.
      if (Math.Abs(initialValue - finalValue) > .01) {
        this.normalX_ = null;
        this.normalY_ = null;
      }
    }
  }
}