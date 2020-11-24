using fin.input.keyboard;

namespace fin.input {
  public interface IInput {
    IGamepad Controller { get; }
    ICursor Cursor { get; }
    IKeyboard Keyboard { get; }

    // TODO: Remove this.
    void Poll();
  }
}