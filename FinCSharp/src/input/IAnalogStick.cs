namespace fin.input {
  using math.geometry;

  public interface IAnalogStick {
    IVector2<float> NormalizedAxes { get; }
  }
}