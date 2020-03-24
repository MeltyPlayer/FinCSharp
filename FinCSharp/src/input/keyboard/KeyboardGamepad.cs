using System.Collections.Generic;

using fin.input.button;

namespace fin.input.keyboard {
  public class KeyboardGamepad : IGamepad {
    private readonly IDictionary<AnalogStickType, IAnalogStick> analogSticks_ =
        new Dictionary<AnalogStickType, IAnalogStick>();

    private readonly IDictionary<FaceButtonType, IButton> faceButtons_ =
        new Dictionary<FaceButtonType, IButton>();

    public KeyboardGamepad(IKeyboard keyboard) {
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