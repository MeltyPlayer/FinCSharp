namespace fin.input {

  public interface IAxis {
    float NormalizedValue { get; }
  }

  public class ButtonAxis : IAxis {
    public IButton Positive { set; get; } = NullButton.Instance;
    public IButton Negative { set; get; } = NullButton.Instance;

    public float NormalizedValue {
      get {
        float positiveValue = (this.Positive.State == ButtonState.DOWN || this.Positive.State == ButtonState.PRESSED) ? 1 : 0;
        float negativeValue = (this.Negative.State == ButtonState.DOWN || this.Negative.State == ButtonState.PRESSED) ? 1 : 0;
        return positiveValue - negativeValue;
      }
    }
  }
}