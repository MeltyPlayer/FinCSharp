using System;

namespace fin.dispose {

  // TODO: Rename this.
  public class UnsafeDisposable : IDisposable {

    public delegate void OnDisposeEventHandler();

    public event OnDisposeEventHandler OnDisposeEvent = delegate { };

    public bool IsDisposed { get; private set; }

    ~UnsafeDisposable() {
      this.Dispose_(false);
    }

    public void Dispose() {
      this.Dispose_(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose_(bool disposing) {
      if (this.IsDisposed) {
        return;
      }
      this.IsDisposed = true;

      if (!disposing) {
        // TODO: Free unmanaged resources here?
        return;
      }

      this.OnDisposeEvent();
    }
  }
}