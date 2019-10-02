namespace fin.input {

  public enum AnalogStickType {
    PRIMARY,
    SECONDARY,
  }

  public enum FaceButtonType {
    PRIMARY,
    SECONDARY,
  }

  public interface IController {
    IAnalogStick this[AnalogStickType analogStickType] { get; }
    IButton this[FaceButtonType faceButtonType] { get; }
  }
}