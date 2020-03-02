namespace fin.input {
  using impl.opentk;

  public interface IInput {
    IController Controller { get; }
    ICursor Cursor { get; }
    IKeyboard Keyboard { get; }
  }
}