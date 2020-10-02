using fin.app.window;
using fin.discardable;

namespace fin.app.impl.opentk {
  using System.Collections.Generic;

  public sealed partial class AppOpenTk {
    private sealed partial class WindowManagerOpenTk : IAppWindowManager {
      private readonly IDiscardableNode node_;
      private readonly AppOpenTk parent_;
      private readonly IList<WindowOpenTk> windows_ = new List<WindowOpenTk>();

      public WindowManagerOpenTk(
          AppOpenTk parentApp,
          IDiscardableNode parentDiscardableNode) {
        this.parent_ = parentApp;
        this.node_ = parentDiscardableNode.CreateChild();
      }

      // TODO: Delete this.
      public IEnumerable<IAppWindow> Windows => this.windows_;

      public void ProcessEvents() {
        foreach (var window in this.windows_) {
          window.ProcessEvents();
        }
      }

      public IAppWindow[] InitWindowsForScene(params IWindowArgs[] args) {
        // TODO: Reuse old windows.
        // TODO: Delete extra windows.
        // TODO: Reset state.
        var count = args.Length;
        var windows = new IAppWindow[count];
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