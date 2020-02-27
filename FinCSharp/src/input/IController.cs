using System.Collections.Generic;

using fin.input.impl.opentk;

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

  public class KeyboardController : IController {
    private readonly IDictionary<AnalogStickType, IAnalogStick> analogSticks_ =
      new Dictionary<AnalogStickType, IAnalogStick>();

    private readonly IDictionary<FaceButtonType, IButton> faceButtons_ =
      new Dictionary<FaceButtonType, IButton>();

    public KeyboardController(IKeyButtonDictionary kbd) {
      this.analogSticks_[AnalogStickType.PRIMARY] = new DpadAnalogStick(
        new ButtonDpad() {
          Up = kbd[KeyId.W],
          Down = kbd[KeyId.S],
          Left = kbd[KeyId.A],
          Right = kbd[KeyId.D]
        });
      this.analogSticks_[AnalogStickType.SECONDARY] = new DpadAnalogStick(
        new ButtonDpad() {
          Up = kbd[KeyId.I],
          Down = kbd[KeyId.K],
          Left = kbd[KeyId.J],
          Right = kbd[KeyId.L]
        });

      this.faceButtons_[FaceButtonType.PRIMARY] = kbd[KeyId.Z];
      this.faceButtons_[FaceButtonType.SECONDARY] = kbd[KeyId.X];
    }

    public IAnalogStick this[AnalogStickType analogStickType] =>
      this.analogSticks_[analogStickType];

    public IButton this[FaceButtonType faceButtonType] =>
      this.faceButtons_[faceButtonType];
  }
}