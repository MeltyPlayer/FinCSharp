using fin.math.geometry;

namespace fin.input.analogstick {
  public interface IAnalogStick {
    IVector2<float> RawAxes { get; }
  }
}