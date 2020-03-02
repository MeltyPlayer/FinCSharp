namespace fin.input {
  public interface IInput {
    IController Controller { get; }
    ICursor Cursor { get; }
  }
}