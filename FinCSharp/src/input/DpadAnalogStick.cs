using fin.math;

namespace fin.input {

  public class DpadAnalogStick : IAnalogStick {
    public IDpad Dpad { get; set; }

    public DpadAnalogStick(IDpad dpad) {
      this.Dpad = dpad;
    }

    public IVector2<float> NormalizedAxes {
      get {
        var xVector = this.GetAxis_(this.Dpad.RightState, this.Dpad.LeftState);
        var yVector = this.GetAxis_(this.Dpad.UpState, this.Dpad.DownState);
        return new ImmutableVector2<float>(xVector, yVector);
      }
    }

    private float GetAxis_(ButtonState positive, ButtonState negative) {
      float positiveValue = (positive == ButtonState.DOWN || positive == ButtonState.PRESSED) ? 1 : 0;
      float negativeValue = (negative == ButtonState.DOWN || negative == ButtonState.PRESSED) ? 1 : 0;
      return positiveValue - negativeValue;
    }
  }
}