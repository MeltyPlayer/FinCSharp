using CMath = System.Math;

namespace fin.math.number {
  public interface IDirection {
    double Fraction { get; set; }

    double Degrees { get; set; }
    double Radians { get; set; }

    double NormalX { get; }
    double NormalY { get; }
    (double, double) Normal { get; set; }
  }

  public class Direction : IDirection {
    private readonly IFraction impl_;

    public double Fraction {
      get => this.impl_.Value;
      set => this.Set_(value);
    }

    public double Degrees {
      get => this.Fraction * 360;
      set => this.Fraction = value / 360;
    }

    public double Radians {
      get => this.impl_.Value * Math.TAU;
      set => this.Fraction = value / Math.TAU;
    }

    private double? normalX_;
    private double? normalY_;

    public double NormalX
      => this.normalX_ ?? (this.normalX_ = CMath.Cos(this.Radians)) ?? 0;

    public double NormalY
      => this.normalY_ ?? (this.normalY_ = CMath.Sin(this.Radians)) ?? 0;

    public (double, double) Normal {
      get => (this.NormalX, this.NormalY);
      set {
        var (normalX, normalY) = value;
        this.Radians = CMath.Atan2(normalY, normalX);
      }
    }

    private Direction() {
      this.impl_ = new CircularFraction(0);
    }

    public static Direction FromDegrees(double degrees) =>
        new Direction() {
            Degrees = degrees,
        };

    public static Direction FromRadians(double radians) =>
        new Direction() {
            Radians = radians,
        };

    public static Direction FromNormal(double normalX, double normalY) =>
        new Direction() {
            Normal = (normalX, normalY),
        };

    private void Set_(double value) {
      var initialValue = this.impl_.Value;
      this.impl_.Value = value;
      var finalValue = this.impl_.Value;

      // TODO: Make sure this tolerance works.
      if (CMath.Abs(initialValue - finalValue) > .01) {
        this.normalX_ = null;
        this.normalY_ = null;
      }
    }
  }
}