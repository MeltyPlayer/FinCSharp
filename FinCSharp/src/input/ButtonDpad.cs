using fin.input.button;

namespace fin.input {
  public class ButtonDpad : IDpad {
    public IButton Up { get; set; } = NullButton.Instance;
    public IButton Down { get; set; } = NullButton.Instance;
    public IButton Left { get; set; } = NullButton.Instance;
    public IButton Right { get; set; } = NullButton.Instance;

    public ButtonState UpState => this.Up.State;

    public ButtonState DownState => this.Down.State;

    public ButtonState LeftState => this.Left.State;

    public ButtonState RightState => this.Right.State;
  }
}