using System.Collections.Generic;
using System.Collections.Immutable;

using fin.input.button;
using fin.input.keyboard;

namespace fin.input.gamepad {
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
      this.AnalogSticks = this.analogSticks_.Values.ToImmutableArray();

      this.faceButtons_[FaceButtonType.PRIMARY] = keyboard[KeyId.U];
      this.faceButtons_[FaceButtonType.SECONDARY] = keyboard[KeyId.H];
      this.faceButtons_[FaceButtonType.START] = keyboard[KeyId.ENTER];
      this.faceButtons_[FaceButtonType.SELECT] = keyboard[KeyId.SHIFT];
      this.Buttons = this.faceButtons_.Values.ToImmutableArray();
    }

    public bool IsConnected => true;

    public ImmutableArray<IAnalogStick> AnalogSticks { get; }
    public IAnalogStick this[AnalogStickType analogStickType] =>
        this.analogSticks_[analogStickType];

    public ImmutableArray<IButton> Buttons { get; }
    public IButton this[FaceButtonType faceButtonType] =>
        this.faceButtons_[faceButtonType];

    public void Poll() {}
  }
}