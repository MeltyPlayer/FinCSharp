using fin.app.window;
using fin.input.button;
using fin.math.geometry;

namespace fin.input.impl.opentk {
  // TODO: Wrap all of the OpenTK stuff inside of AppOpenTk?
  public class CursorOpenTk : ICursor {
    public CursorOpenTk(ButtonManagerOpenTk buttonManager) {
      this.LeftButton = buttonManager.New();
      this.RightButton = buttonManager.New();
    }

    /// <summary>
    ///   Public set is hidden behind the interface.
    /// </summary>
    public IWindow? Window { get; set; }

    IVector2<int>? ICursor.Position =>
        this.Window != null ? this.Position : null;

    public MutableVector2<int> Position { get; } = new MutableVector2<int>();

    IButton ICursor.LeftButton => this.LeftButton;
    public IButtonImplementation LeftButton { get; }

    IButton ICursor.RightButton => this.RightButton;
    public IButtonImplementation RightButton { get; }
  }
}