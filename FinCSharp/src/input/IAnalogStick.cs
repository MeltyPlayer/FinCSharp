using fin.math;

namespace fin.input {
  public interface IAnalogStick {
    fin.math.geometry.IVector2<float> NormalizedAxes { get; }
  }
}