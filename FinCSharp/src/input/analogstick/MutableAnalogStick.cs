using fin.math.geometry;

namespace fin.input.analogstick {
  public class MutableAnalogStick : IAnalogStick {
    public MutableVector2<float> RawAxes { get; } = new MutableVector2<float>();

    IVector2<float> IAnalogStick.RawAxes => this.RawAxes;
  }
}