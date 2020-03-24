namespace fin.input {
  using button;

  public enum AnalogStickType {
    PRIMARY,
    SECONDARY,
  }

  public enum FaceButtonType {
    PRIMARY,
    SECONDARY,
  }

  public interface IGamepad {
    IAnalogStick this[AnalogStickType analogStickType] { get; }
    IButton this[FaceButtonType faceButtonType] { get; }
  }
}