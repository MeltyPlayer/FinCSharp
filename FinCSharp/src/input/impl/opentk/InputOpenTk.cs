namespace fin.input.impl.opentk {
  using fin.input;

  public class InputOpenTk : IInput {
    public InputOpenTk() {
      this.Cursor = new CursorOpenTk(this.ButtonManager);
      this.Keyboard = new KeyboardOpenTk(this.ButtonManager);

      this.Controller = new KeyboardController(this.Keyboard);
    }

    public ButtonManagerOpenTk ButtonManager { get; } =
      new ButtonManagerOpenTk();

    public IController Controller { get; }

    ICursor IInput.Cursor => this.Cursor;
    public CursorOpenTk Cursor { get; }

    IKeyboard IInput.Keyboard => this.Keyboard; 
    public KeyboardOpenTk Keyboard { get; }
  }
}