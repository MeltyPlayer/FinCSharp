using System.Collections.Generic;
using System.Collections.Immutable;

using fin.input.analogstick;
using fin.input.button;
using fin.input.gamepad;

using OpenTK.Input;

namespace fin.input.impl.opentk {
  public class GamepadOpenTk : IGamepad {
    public bool IsConnected { get; private set; } = true;

    private readonly MutableAnalogStick primaryAnalogStick_ =
        new MutableAnalogStick();

    private readonly MutableAnalogStick secondaryAnalogStick_ =
        new MutableAnalogStick();

    private readonly IButtonImplementation primaryButton_;
    private readonly IButtonImplementation secondaryButton_;

    public GamepadOpenTk(
        GamePadCapabilities capabilities,
        IButtonManager buttonManager) {
      this.AnalogSticks = new IAnalogStick[]
              {this.primaryAnalogStick_, this.secondaryAnalogStick_}
          .ToImmutableArray();

      this.primaryButton_ = buttonManager.New();
      this.secondaryButton_ = buttonManager.New();
      this.Buttons = new IButton[] {this.primaryButton_, this.secondaryButton_}
          .ToImmutableArray();
    }

    public void UpdateState(GamePadState state) {
      this.IsConnected = state.IsConnected;

      var leftThumbStick = state.ThumbSticks.Left;
      var primaryAxes = this.primaryAnalogStick_.RawAxes;
      primaryAxes.X = leftThumbStick.X;
      primaryAxes.Y = leftThumbStick.Y;

      var rightThumbStick = state.ThumbSticks.Right;
      var secondaryAxes = this.secondaryAnalogStick_.RawAxes;
      secondaryAxes.X = rightThumbStick.X;
      secondaryAxes.Y = rightThumbStick.Y;


      if (state.Buttons.LeftShoulder == OpenTK.Input.ButtonState.Pressed) {
        this.primaryButton_.Down();
      } else {
        this.primaryButton_.Up();
      }

      if (state.Buttons.RightShoulder == OpenTK.Input.ButtonState.Pressed) {
        this.secondaryButton_.Down();
      } else {
        this.secondaryButton_.Up();
      }
    }

    public ImmutableArray<IAnalogStick> AnalogSticks { get; }

    public IAnalogStick this[AnalogStickType analogStickType]
      => analogStickType switch {
          AnalogStickType.PRIMARY   => this.primaryAnalogStick_,
          AnalogStickType.SECONDARY => this.secondaryAnalogStick_,
      };

    public ImmutableArray<IButton> Buttons { get; }

    public IButton this[FaceButtonType faceButtonType]
      => faceButtonType switch {
          FaceButtonType.PRIMARY   => this.primaryButton_,
          FaceButtonType.SECONDARY => this.secondaryButton_,
      };

    public void Poll() { }
  }

  public class GamepadManagerOpenTk {
    private readonly IButtonManager buttonManager_;

    private readonly IDictionary<string, GamepadOpenTk> gamepads_ =
        new Dictionary<string, GamepadOpenTk>();

    public GamepadManagerOpenTk(IButtonManager buttonManager) {
      this.buttonManager_ = buttonManager;
      this.Poll();
    }

    public void Poll() {
      string name;
      for (var i = 0; (name = GamePad.GetName(i)) != ""; ++i) {
        if (!this.gamepads_.TryGetValue(name, out var gamepad)) {
          // It is assumed that gamepad capabilities never change.
          var capabilities = GamePad.GetCapabilities(i);

          gamepad = new GamepadOpenTk(capabilities, this.buttonManager_);
          this.gamepads_[name] = gamepad;
        }

        var state = GamePad.GetState(i);
        gamepad!.UpdateState(state);
      }
    }

    public IEnumerable<IGamepad> All => this.gamepads_.Values;
  }
}