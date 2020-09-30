using fin.math.geometry;

namespace fin.app.window {
  public interface IWindowArgs {
    IDimensions<int> Dimensions { get; }
  }

  public class WindowArgs : IWindowArgs {
    public IDimensions<int> Dimensions => this.dimensions_;

    private readonly MutableDimensions<int> dimensions_ =
      new MutableDimensions<int>();

    public WindowArgs SetDimensions(IDimensions<int> dimensions) {
      (this.dimensions_.Width, this.dimensions_.Height) = (dimensions.Width,
                                                           dimensions.Height);
      return this;
    }
  }

  public interface IAppWindowManager {
    /// <summary>
    ///   Creates a new window.
    /// </summary>
    public IAppWindow NewWindow(IWindowArgs args);

    /// <summary>
    ///   Initializes windows for the current scene such that {count} exist.
    ///   Creates windows if none exist, reuses existing ones if available,
    ///   closes some if there are too many.
    /// </summary>
    public IAppWindow[] InitWindowsForScene(params IWindowArgs[] args);
  }
}