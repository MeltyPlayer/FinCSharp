using System.Linq;

using fin.data.enumerator;
using fin.input.button;
using fin.input.gamepad;
using fin.input.keyboard;

namespace fin.input.impl.opentk {
  public class InputOpenTk : IInput {
    private readonly ButtonManagerOpenTk buttonManager_ =
        new ButtonManagerOpenTk();

    private readonly JoystickManagerOpenTk joystickManager_;
    private readonly GamepadManagerOpenTk gamepadManager_;

    public InputOpenTk() {
      this.Cursor = new CursorOpenTk(this.buttonManager_);
      this.Keyboard = new KeyboardOpenTk(this.buttonManager_);

      this.joystickManager_ = new JoystickManagerOpenTk(this.buttonManager_);
      this.gamepadManager_ = new GamepadManagerOpenTk(this.buttonManager_);

      var gamepads = new AggregateEnumerable<IGamepad>(
          new IGamepad[] {
              new KeyboardGamepad(this.Keyboard),
              new JoystickGamepad(this.joystickManager_.All.First())
          },
          this.gamepadManager_.All);

      this.Controller = new AggregateGamepad(gamepads);
    }

    public IGamepad Controller { get; }

    ICursor IInput.Cursor => this.Cursor;
    public CursorOpenTk Cursor { get; }

    IKeyboard IInput.Keyboard => this.Keyboard;
    public KeyboardOpenTk Keyboard { get; }

    public void Poll() {
      this.joystickManager_.Poll();
      this.buttonManager_.HandleTransitions();
      this.Controller.Poll();
    }
  }
}