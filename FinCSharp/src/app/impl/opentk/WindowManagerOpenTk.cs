using fin.app.window;

namespace fin.app.impl.opentk {

  public sealed partial class AppOpenTk : IApp {

    private sealed partial class WindowManagerOpenTk : IWindowManager {
      private readonly AppOpenTk parent_;

      public WindowManagerOpenTk(AppOpenTk parent) {
        this.parent_ = parent;
      }

      public IWindow[] InitWindowsForScene(params IWindowArgs[] args) {
        // TODO: Reuse old windows.
        // TODO: Delete extra windows.
        // TODO: Reset state.
        var count = args.Length;
        var windows = new IWindow[count];
        for (var i = 0; i < count; ++i) {
          windows[i] = this.NewWindow(args[i]);
        }
        return windows;
      }
    }
  }
}