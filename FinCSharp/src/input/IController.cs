namespace fin.input {
  using System.Collections.Generic;

  using button;

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

  public class KeyboardController : IController {
    private readonly IDictionary<AnalogStickType, IAnalogStick> analogSticks_ =
        new Dictionary<AnalogStickType, IAnalogStick>();

    private readonly IDictionary<FaceButtonType, IButton> faceButtons_ =
        new Dictionary<FaceButtonType, IButton>();

    public KeyboardController(IKeyboard keyboard) {
      this.analogSticks_[AnalogStickType.PRIMARY] = new DpadAsAnalogStick(
          new ButtonDpad() {
              Up = keyboard[KeyId.W],
              Down = keyboard[KeyId.S],
              Left = keyboard[KeyId.A],
              Right = keyboard[KeyId.D]
          });
      this.analogSticks_[AnalogStickType.SECONDARY] = new DpadAsAnalogStick(
          new ButtonDpad() {
              Up = keyboard[KeyId.I],
              Down = keyboard[KeyId.K],
              Left = keyboard[KeyId.J],
              Right = keyboard[KeyId.L]
          });

      this.faceButtons_[FaceButtonType.PRIMARY] = keyboard[KeyId.Z];
      this.faceButtons_[FaceButtonType.SECONDARY] = keyboard[KeyId.X];
    }

    public IAnalogStick this[AnalogStickType analogStickType] =>
        this.analogSticks_[analogStickType];

    public IButton this[FaceButtonType faceButtonType] =>
        this.faceButtons_[faceButtonType];
  }
}