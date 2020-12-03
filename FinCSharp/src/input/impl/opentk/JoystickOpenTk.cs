using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using fin.input.button;

using OpenTK.Input;

namespace fin.input.impl.opentk {
  // TODO: Perform some of this in a manager class.
  public class JoystickOpenTk : IJoystick {
    public bool IsConnected { get; private set; } = true;

    private readonly MutableAxis[] axes_;
    public ImmutableArray<IAxis> Axes { get; }

    private readonly IButtonImplementation[] buttons_;
    public ImmutableArray<IButton> Buttons { get; }

    public JoystickOpenTk(
        JoystickCapabilities capabilities,
        IButtonManager buttonManager) {
      this.axes_ = new MutableAxis[capabilities.AxisCount];
      for (var i = 0; i < this.axes_.Length; ++i) {
        this.axes_[i] = new MutableAxis();
      }
      this.Axes = this.axes_.ToImmutableArray<IAxis>();

      this.buttons_ = new IButtonImplementation[capabilities.ButtonCount];
      for (var i = 0; i < this.buttons_.Length; ++i) {
        this.buttons_[i] = buttonManager.New();
      }
      this.Buttons = this.buttons_.ToImmutableArray<IButton>();
    }

    public void UpdateState(JoystickState state) {
      this.IsConnected = state.IsConnected;

      for (var i = 0; i < this.axes_.Length; ++i) {
        var axis = this.axes_[i];
        axis.Value = state.GetAxis(i);
      }

      for (var i = 0; i < this.buttons_.Length; ++i) {
        var button = this.buttons_[i];

        var buttonState = state.GetButton(i);
        if (buttonState == OpenTK.Input.ButtonState.Pressed) {
          button.Down();
        } else {
          button.Up();
        }
      }
    }
  }

  public class JoystickManagerOpenTk {
    private readonly IButtonManager buttonManager_;

    private readonly IDictionary<Guid, JoystickOpenTk> joysticks_ =
        new Dictionary<Guid, JoystickOpenTk>();

    public JoystickManagerOpenTk(IButtonManager buttonManager) {
      this.buttonManager_ = buttonManager;
      this.Poll();
    }

    public void Poll() {
      JoystickState state;
      for (var i = 0; (state = Joystick.GetState(i)).IsConnected; ++i) {
        var guid = Joystick.GetGuid(i);

        if (!this.joysticks_.TryGetValue(guid, out var joystick)) {
          // It is assumed that joystick capabilities never change.
          var capabilities = Joystick.GetCapabilities(i);

          joystick = new JoystickOpenTk(capabilities, this.buttonManager_);
          this.joysticks_[guid] = joystick;
        }

        joystick!.UpdateState(state);
      }
    }

    public IEnumerable<IJoystick> All => this.joysticks_.Values;
  }
}