namespace fin.input.keyboard {
  using button;

  public interface IKeyboard {
    IButton this[KeyId keyId] { get; }
  }
}