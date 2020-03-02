namespace fin.input {
  using app.window;

  using math.geometry;

  public interface ICursor {
    IWindow? Window { get; }
    IVector2<int>? Position { get; }
  }
}
