namespace fin.input {
  using button;

  public interface IDpad {
    ButtonState UpState { get; }
    ButtonState DownState { get; }
    ButtonState LeftState { get; }
    ButtonState RightState { get; }
  }
}