using fin.input.button;
using fin.math.geometry;

namespace fin.input.analogstick {
  public class ButtonAxis : IAxis {
    public IButton Positive { set; get; } = NullButton.Instance;
    public IButton Negative { set; get; } = NullButton.Instance;

    public float Value {
      get {
        float positiveValue =
            (this.Positive.State == ButtonState.DOWN ||
             this.Positive.State == ButtonState.PRESSED)
                ? 1
                : 0;
        float negativeValue =
            (this.Negative.State == ButtonState.DOWN ||
             this.Negative.State == ButtonState.PRESSED)
                ? 1
                : 0;
        return positiveValue - negativeValue;
      }
    }
  }

  // TODO: Use AxesAsAnalogStick as impl.
  public class DpadAsAnalogStick : IAnalogStick {
    public IDpad Dpad { get; set; }

    public DpadAsAnalogStick(IDpad dpad) {
      this.Dpad = dpad;
    }

    public IVector2<float> RawAxes {
      get {
        var xVector = this.GetAxis_(this.Dpad.RightState, this.Dpad.LeftState);
        var yVector = this.GetAxis_(this.Dpad.UpState, this.Dpad.DownState);
        return new ImmutableVector2<float>(xVector, yVector);
      }
    }

    private float GetAxis_(ButtonState positive, ButtonState negative) {
      float positiveValue =
          (positive == ButtonState.DOWN || positive == ButtonState.PRESSED)
              ? 1
              : 0;
      float negativeValue =
          (negative == ButtonState.DOWN || negative == ButtonState.PRESSED)
              ? 1
              : 0;
      return positiveValue - negativeValue;
    }
  }
}