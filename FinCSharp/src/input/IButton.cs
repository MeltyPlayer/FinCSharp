namespace fin.input {
  public enum ButtonState {
    DOWN,
    PRESSED,
    UP,
  }

  public interface IButton {
    bool IsUp();
    bool IsDown();
    bool IsPressed();
  }
}