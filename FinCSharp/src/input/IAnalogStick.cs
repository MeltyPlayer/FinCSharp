using fin.math;

namespace fin.input {

  public interface IAnalogStick {
    IVector2 NormalizedAxes { get; }
  }
}