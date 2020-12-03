using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using fin.input.analogstick;
using fin.input.button;
using fin.math;

namespace fin.input.gamepad {
  public class AggregateGamepad : IGamepad {
    private readonly IEnumerable<IGamepad> gamepads_;
    private IGamepad activeGamepad_;

    public AggregateGamepad(IEnumerable<IGamepad> gamepads) {
      this.gamepads_ = gamepads;
      this.activeGamepad_ = gamepads.First();
    }

    public bool IsConnected => this.activeGamepad_.IsConnected;

    public ImmutableArray<IAnalogStick> AnalogSticks
      => this.activeGamepad_.AnalogSticks;

    public IAnalogStick this[AnalogStickType analogStickType]
      => this.activeGamepad_[analogStickType];

    public ImmutableArray<IButton> Buttons => this.activeGamepad_.Buttons;

    public IButton this[FaceButtonType faceButtonType]
      => this.activeGamepad_[faceButtonType];

    public void Poll() {
      var touchedGamepad = this.FindFirstTouchedGamepad_();
      if (touchedGamepad != null) {
        this.activeGamepad_ = touchedGamepad;
      }
    }

    private IGamepad? FindFirstTouchedGamepad_() {
      foreach (var gamepad in this.gamepads_) {
        foreach (var analogStick in gamepad.AnalogSticks) {
          var rawAxes = analogStick.RawAxes;
          if (FloatMath.Abs(rawAxes.X) > GamepadConstants.DEADZONE ||
              FloatMath.Abs(rawAxes.Y) > GamepadConstants.DEADZONE) {
            return gamepad;
          }
        }

        foreach (var button in gamepad.Buttons) {
          if (button.IsDown) {
            return gamepad;
          }
        }
      }

      return null;
    }
  }
}