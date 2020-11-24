using fin.input.gamepad;
using fin.input.keyboard;

namespace fin.input.impl.opentk {
  public class InputOpenTk : IInput {
    private readonly JoystickManagerOpenTk joystickManager_;

    private readonly ButtonManagerOpenTk buttonManager_ =
        new ButtonManagerOpenTk();

    public InputOpenTk() {
      this.Cursor = new CursorOpenTk(this.buttonManager_);
      this.Keyboard = new KeyboardOpenTk(this.buttonManager_);

      this.joystickManager_ = new JoystickManagerOpenTk(this.buttonManager_);

      this.Controller = new AggregateGamepad(
          new KeyboardGamepad(this.Keyboard),
          new JoystickGamepad(this.joystickManager_.First));
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