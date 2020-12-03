using fin.math.geometry;

namespace fin.input.analogstick {
  public class AxesAsAnalogStick : IAnalogStick {
    public IAxis Horizontal {
      get => this.axes_.Horizontal;
      set => this.axes_.Horizontal = value;
    }

    public IAxis Vertical {
      get => this.axes_.Vertical;
      set => this.axes_.Vertical = value;
    }

    private readonly AxesVector2 axes_ = new AxesVector2();
    public IVector2<float> RawAxes => this.axes_;

    private class AxesVector2 : IVector2<float> {
      public IAxis Horizontal { get; set; }
      public IAxis Vertical { get; set; }

      public float X => this.Horizontal.Value;

      // Needs to be negative for joystick axes.
      // TODO: Make this a more specified rule.
      public float Y => -this.Vertical.Value;
    }
  }
}