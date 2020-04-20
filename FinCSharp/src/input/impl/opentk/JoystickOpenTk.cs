using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using fin.input;
using fin.input.button;
using OpenTK;
using OpenTK.Input;
using OpenTK.Platform.Windows;

namespace fin.input.impl.opentk {
  // TODO: Perform some of this in a manager class.
  public class JoystickOpenTk : IJoystick {
    public bool IsConnected { get; private set; } = true;


    //private readonly IAnalogStick[] analogSticks_;
    public ImmutableArray<IAnalogStick> AnalogSticks => throw new NotImplementedException();

    //private readonly IDpad[] dpads_;
    public ImmutableArray<IDpad> Dpads => throw new NotImplementedException();


    private readonly IButtonImplementation[] buttons_;
    public ImmutableArray<IButton> Buttons { get; private set; }


    public JoystickOpenTk(JoystickCapabilities capabilities, IButtonManager buttonManager) {
      this.buttons_ = new IButtonImplementation[capabilities.ButtonCount];
      this.Buttons = this.buttons_.ToImmutableArray<IButton>();
    }

    public void UpdateState(JoystickState state) {
      this.IsConnected = state.IsConnected;

      for (int i = 0; i < this.buttons_.Length; ++i) {
        var button = this.buttons_[i];
       
        var buttonState = state.GetButton(i);
        if (buttonState == OpenTK.Input.ButtonState.Pressed) {
          button.Down();
        }
        else {
          button.Up();
        }
      }
    }
  }

  public class JoystickManagerOpenTk {
    private readonly IButtonManager buttonManager_;
    private readonly IDictionary<Guid, JoystickOpenTk> joysticks_ = new Dictionary<Guid, JoystickOpenTk>();

    public JoystickManagerOpenTk(IButtonManager buttonManager) {
      this.buttonManager_ = buttonManager;
    }

    public void Poll() {
      JoystickState state;
      for (int i = 0; (state = Joystick.GetState(i)).IsConnected; ++i) {
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
  }
}
