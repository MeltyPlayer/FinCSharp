using fin.app.window;

namespace fin.app.impl.opentk {
  using System.Collections.Generic;

  public sealed partial class AppOpenTk {
    private sealed partial class WindowManagerOpenTk : IWindowManager {
      private readonly AppOpenTk parent_;
      private readonly IList<WindowOpenTk> windows_ = new List<WindowOpenTk>();

      public WindowManagerOpenTk(AppOpenTk parent) {
        this.parent_ = parent;
      }

      public void ProcessEvents() {
        foreach (var window in this.windows_) {
          window.ProcessEvents();
        }
      }

      public IWindow[] InitWindowsForScene(params IWindowArgs[] args) {
        // TODO: Reuse old windows.
        // TODO: Delete extra windows.
        // TODO: Reset state.
        var count = args.Length;
        var windows = new IWindow[count];
        for (var i = 0; i < count; ++i) {
          var window = this.NewWindow(args[i]);
          windows[i] = window;
          this.windows_.Add((WindowOpenTk) window);
        }

        return windows;
      }
    }
  }
}