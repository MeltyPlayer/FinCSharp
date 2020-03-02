namespace fin.input {
  using app.window;

  using button;

  using math.geometry;

  public interface ICursor {
    IWindow? Window { get; }
    IVector2<int>? Position { get; }

    IButton LeftButton { get; }
    IButton RightButton { get; }
  }
}
