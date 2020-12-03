using System.Collections.Generic;
using System.Collections.Immutable;

using fin.input.analogstick;
using fin.input.button;

namespace fin.input.gamepad {
  public class JoystickGamepad : IGamepad {
    private readonly IJoystick joystick_;

    private readonly IDictionary<AnalogStickType, IAnalogStick> analogSticks_ =
        new Dictionary<AnalogStickType, IAnalogStick>();

    private readonly IDictionary<FaceButtonType, IButton> faceButtons_ =
        new Dictionary<FaceButtonType, IButton>();

    public JoystickGamepad(IJoystick joystick) {
      this.joystick_ = joystick;

      // TODO: Consistent way to get analog sticks?
      var axes = joystick.Axes;
      this.analogSticks_[AnalogStickType.PRIMARY] =
          new AxesAsAnalogStick {Horizontal = axes[0], Vertical = axes[1]};
      this.analogSticks_[AnalogStickType.SECONDARY] =
          new AxesAsAnalogStick {Horizontal = axes[3], Vertical = axes[4]};
      this.AnalogSticks = this.analogSticks_.Values.ToImmutableArray();

      // TODO: Consistent way to get buttons?
      var buttons = joystick.Buttons;
      this.faceButtons_[FaceButtonType.PRIMARY] = buttons[4];
      this.faceButtons_[FaceButtonType.SECONDARY] = buttons[5];
      this.faceButtons_[FaceButtonType.START] = buttons[8];
      this.faceButtons_[FaceButtonType.SELECT] = buttons[7];
      this.Buttons = this.faceButtons_.Values.ToImmutableArray();
    }

    public bool IsConnected => this.joystick_.IsConnected;

    public ImmutableArray<IAnalogStick> AnalogSticks { get; }
    public IAnalogStick this[AnalogStickType analogStickType] =>
        this.analogSticks_[analogStickType];

    public ImmutableArray<IButton> Buttons { get; }
    public IButton this[FaceButtonType faceButtonType] =>
        this.faceButtons_[faceButtonType];

    public void Poll() { }
  }
}