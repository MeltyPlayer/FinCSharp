namespace fin.input {
  using app.window;

  using button;

  using math.geometry;

  public interface ICursor {
    IAppWindow? Window { get; }

    /// <summary>
    ///   The current (x, y) location of the cursor.
    /// </summary>
    /// <remarks>
    ///   Is null if the cursor is not over a window.
    /// </remarks>
    IVector2<int>? Position { get; }

    IButton LeftButton { get; }
    IButton RightButton { get; }
  }
}