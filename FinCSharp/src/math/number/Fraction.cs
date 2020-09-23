namespace fin.math.number {
  public interface IFraction : IRangedNumber<double> {}

  public abstract class BFraction : IFraction {
    private readonly IRangedNumber<double> impl_;

    public double Min => this.impl_.Min;

    public double Value {
      get => this.impl_.Value;
      set => this.impl_.Value = value;
    }

    public double Max => this.impl_.Max;

    protected BFraction(IRangedNumber<double> impl) {
      this.impl_ = impl;
    }
  }

  public class ClampedFraction : BFraction {
    public ClampedFraction(double initialValue) : base(
        new ClampedRangedNumber<double>(0, initialValue, 1)) {}
  }

  public sealed class CircularFraction : BFraction {
    public CircularFraction(double initialValue) : base(
        new CircularRangedNumber<double>(0, initialValue, 1)) { }
  }
}