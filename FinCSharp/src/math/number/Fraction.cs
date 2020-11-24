namespace fin.math.number {
  public interface IFraction : IRangedFloat {}

  public abstract class BFraction : IFraction {
    private readonly IRangedFloat impl_;

    public float Min => this.impl_.Min;

    public float Value {
      get => this.impl_.Value;
      set => this.impl_.Value = value;
    }

    public float Max => this.impl_.Max;

    protected BFraction(IRangedFloat impl) {
      this.impl_ = impl;
    }
  }

  public class ClampedFraction : BFraction {
    public ClampedFraction(float initialValue) : base(
        new ClampedRangedFloat(0, initialValue, 1)) {}
  }

  public sealed class CircularFraction : BFraction {
    public CircularFraction(float initialValue) : base(
        new CircularRangedFloat(0, initialValue, 1)) { }
  }
}