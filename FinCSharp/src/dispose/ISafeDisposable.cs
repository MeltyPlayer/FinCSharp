using System;
using System.Collections.Generic;
using System.Linq;
using fin.assert;

namespace fin.dispose {
  // TODO: Rename this.
  public class UnsafeDisposable : IDisposable {
    public delegate void OnDisposeEventHandler();

    public event OnDisposeEventHandler OnDisposeEvent;
    public bool IsDisposed { get; private set; }

    ~UnsafeDisposable() {
      this.Dispose(false);
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
      if (this.IsDisposed) {
        return;
      }
      this.IsDisposed = true;

      if (!disposing) {
        // TODO: Free unmanaged resources here?
        return;
      }

      this.OnDisposeEvent?.Invoke();
    }
  }

  public class UnsafeDisposableDataNode<T> : UnsafeDisposable {
    public T Data { get; }

    public UnsafeDisposableDataNode<T> Parent { get; private set; }
    public ISet<UnsafeDisposableDataNode<T>> Children { get; }
    public ISet<T> ChildData { get; }

    public UnsafeDisposableDataNode(T data, UnsafeDisposableDataNode<T> parent) {
      this.Data = data;
      parent?.Attach(this);

      this.Children = new HashSet<UnsafeDisposableDataNode<T>>();
      this.ChildData = new HashSet<T>();

      this.OnDisposeEvent += this.OnDispose;
    }

    private void OnDispose() {
      this.Parent?.RemoveSingle(this);

      foreach (var child in this.Children) {
        child.Dispose();
      }

      this.Children.Clear();
      this.ChildData.Clear();
    }

    public UnsafeDisposableDataNode<T> Attach(
      params UnsafeDisposableDataNode<T>[] children) {
      foreach (var child in children) {
        this.AttachSingle(child);
      }
      return this;
    }

    private void AttachSingle(UnsafeDisposableDataNode<T> child) {
      Assert.False(this.IsDisposed);
      Assert.Different(this, child);
      Assert.Nonnull(child);

      if (this.Children.Contains(child)) {
        return;
      }

      this.Children.Add(child);
      this.ChildData.Add(child.Data);

      child.Parent?.RemoveSingle(child);
      child.Parent = this;
    }

    private void RemoveSingle(UnsafeDisposableDataNode<T> child) {
      Assert.False(this.IsDisposed);
      Assert.Different(this, child);
      Assert.Nonnull(child);

      if (!this.Children.Contains(child)) {
        return;
      }

      this.Children.Remove(child);
      this.ChildData.Remove(child.Data);

      child.Parent = null;
    }

    public override bool Equals(object obj) {
      return this.Equals(obj as UnsafeDisposableDataNode<T>);
    }

    public bool Equals(UnsafeDisposableDataNode<T> other) {
      // If parameter is null, return false.
      if (Object.ReferenceEquals(other, null)) {
        return false;
      }

      // Optimization for a common success case.
      if (Object.ReferenceEquals(this, other)) {
        return true;
      }

      // Return true if the fields match.
      // Note that the base class is not invoked because it is
      // System.Object, which defines Equals as reference equality.
      return this.Data.Equals(other.Data);
    }

    public override int GetHashCode() {
      return this.Data.GetHashCode();
    }

    public static bool operator ==(
      UnsafeDisposableDataNode<T> lhs,
      UnsafeDisposableDataNode<T> rhs) {
      // Check for null on left side.
      if (Object.ReferenceEquals(lhs, null)) {
        if (Object.ReferenceEquals(rhs, null)) {
          // null == null = true.
          return true;
        }

        // Only the left side is null.
        return false;
      }
      // Equals handles case of null on right side.
      return lhs.Data.Equals(rhs.Data);
    }

    public static bool operator !=(
      UnsafeDisposableDataNode<T> lhs,
      UnsafeDisposableDataNode<T> rhs) {
      return !(lhs == rhs);
    }
  }

  public class SafeDisposableNode {
    protected delegate void OnDisposeEventHandler();

    protected event OnDisposeEventHandler OnDisposeEvent;
    private readonly UnsafeDisposableDataNode<SafeDisposableNode> impl_;

    // TODO: Switch out null for the parent based on current scope.
    public SafeDisposableNode(SafeDisposableNode parent = null) {
      this.impl_ = new UnsafeDisposableDataNode<SafeDisposableNode>(this,
        parent?.impl_);

      this.impl_.OnDisposeEvent += this.OnDispose;
    }

    protected void TriggerDispose() => this.impl_.Dispose();

    private void OnDispose() {
      this.OnDisposeEvent?.Invoke();
    }

    public SafeDisposableNode Parent => this.impl_.Parent.Data;
    public ISet<SafeDisposableNode> Children => this.impl_.ChildData;

    public SafeDisposableNode Attach(params SafeDisposableNode[] children) {
      foreach (var child in children) {
        this.impl_.Attach(child.impl_);
      }
      return this;
    }
  }
}