using System;
using System.Collections.Generic;
using fin.assert;

namespace fin.dispose {
  // TODO: Rename this.
  public abstract class IFinDisposable : IDisposable {
    protected delegate void OnDisposeEventHandler();
    protected event OnDisposeEventHandler OnDisposeEvent;
    private bool isDisposed_;

    ~IFinDisposable() {
      this.Dispose(false);
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
      if (this.isDisposed_) {
        return;
      }
      this.isDisposed_ = true;

      if (!disposing) {
        // TODO: Free unmanaged resources here?
        return;
      }

      this.OnDisposeEvent?.Invoke();
    }

    public bool IsDisposed() {
      return this.isDisposed_;
    }
  }

  /*public abstract class IDisposableParent : IFinDisposable {
    private readonly ISet<ISafeDisposable> children_;

    protected IDisposableParent() {
      this.children_ = new HashSet<ISafeDisposable>();
      this.OnDisposeEvent += this.OnDispose;
    }

    private void OnDispose() {
      foreach (var child in this.children_) {
        child.Dispose();
      }
      this.children_.Clear();
    }

    protected IDisposableParent Attach(params ISafeDisposable[] children) {
      foreach (var child in children) {
        this.AttachSingle(child);
      }
      return this;
    }

    private void AttachSingle(ISafeDisposable child) {
      Assert.Different(this, child);
      Assert.Nonnull(child);

      if (this.children_.Contains(child)) {
        return;
      }

      this.children_.Add(child);
      child.GetParent()?.RemoveSingle(child);
      child.parent_ = this;
    }

    private void RemoveSingle(ISafeDisposable child) {
      Assert.Different(this, child);
      Assert.Nonnull(child);

      if (!this.children_.Contains(child)) {
        return;
      }

      this.children_.Remove(child);
      child.parent_ = null;
    }
  }

  public abstract class ISafeDisposable : IDisposableParent {
    private IDisposableParent parent_;

    protected ISafeDisposable(IDisposableParent parent) {
      this.parent_.Attach(this);
      this.OnDisposeEvent += this.OnDispose;
    }

    private void OnDispose() {
      this.parent_.RemoveSingle(this);
    }

    public IDisposableParent GetParent() {
      return this.parent_;
    }
  }*/
}