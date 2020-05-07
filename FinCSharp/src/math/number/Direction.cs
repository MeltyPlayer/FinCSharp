using CMath = System.Math;

namespace fin.math.number {
  public interface IDirection {
    double Degrees { get; set; }
    double Radians { get; set; }

    double X { get; }
    double Y { get; }
  }

  public class Direction : IDirection {
    private readonly IFraction impl_;

    public double Degrees {
      get => this.impl_.Value * 360;
      set => this.Set_(value / 360);
    }

    public double Radians {
      get => this.impl_.Value * Math.TAU;
      set => this.Set_(value / Math.TAU);
    }

    public double X { get; private set; }
    public double Y { get; private set; }

    private Direction(double value) {
      this.impl_ = new CircularFraction(0);
      this.Set_(value);
    }

    public static Direction FromDegrees(double degrees) {
      var direction = new Direction(degrees);
      direction.Degrees = degrees;
      return direction;
    }

    public static Direction FromRadians(double radians) {
      var direction = new Direction(radians);
      direction.Radians = radians;
      return direction;
    }

    private void Set_(double value) {
      this.impl_.Value = value;

      var radians = this.Radians;
      this.X = CMath.Cos(radians);
      this.Y = CMath.Sin(radians);
    }
  }
}