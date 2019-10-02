namespace fin.input {

  public interface IDpad {
    ButtonState UpState { get; }
    ButtonState DownState { get; }
    ButtonState LeftState { get; }
    ButtonState RightState { get; }
  }
}