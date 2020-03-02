namespace fin.input.button {
  public interface IButton {
    ButtonState State { get; }

    bool IsPressed => this.State == ButtonState.PRESSED;
    bool IsReleased => this.State == ButtonState.RELEASED;

    bool IsDown => this.State == ButtonState.PRESSED ||
                   this.State == ButtonState.DOWN;

    bool IsUp => this.State == ButtonState.RELEASED ||
                 this.State == ButtonState.UP;
  }
}