using System;
using System.Collections.Generic;
using fin.assert;

namespace fin.discard {

  public class UnsafeDisposableDataNode<T> : IDiscardable {
    public T Data { get; }

    public UnsafeDisposableDataNode<T>? Parent { get; private set; }
    public ISet<UnsafeDisposableDataNode<T>> Children { get; }
    public ISet<T> ChildData { get; }

    public UnsafeDisposableDataNode(T data, UnsafeDisposableDataNode<T>? parent) {
      this.Data = data;
      parent?.Attach(this);

      this.Children = new HashSet<UnsafeDisposableDataNode<T>>();
      this.ChildData = new HashSet<T>();

      this.OnDiscardEvent += this.OnDispose_;
    }

    private void OnDispose_() {
      this.Parent?.RemoveSingle_(this);

      foreach (var child in this.Children) {
        child.Discard();
      }

      this.Children.Clear();
      this.ChildData.Clear();
    }

    public UnsafeDisposableDataNode<T> Attach(
      params UnsafeDisposableDataNode<T>[] children) {
      foreach (var child in children) {
        this.AttachSingle_(child);
      }
      return this;
    }

    private void AttachSingle_(UnsafeDisposableDataNode<T> child) {
      Asserts.False(this.IsDiscarded);
      Asserts.Different(this, child);
      Asserts.Nonnull(child);

      if (this.Children.Contains(child)) {
        return;
      }

      this.Children.Add(child);
      this.ChildData.Add(child.Data);

      child.Parent?.RemoveSingle_(child);
      child.Parent = this;
    }

    private void RemoveSingle_(UnsafeDisposableDataNode<T> child) {
      Asserts.False(this.IsDiscarded);
      Asserts.Different(this, child);
      Asserts.Nonnull(child);

      if (!this.Children.Contains(child)) {
        return;
      }

      this.Children.Remove(child);
      this.ChildData.Remove(child.Data);

      child.Parent = null;
    }

    public override bool Equals(object? obj) {
      return obj is UnsafeDisposableDataNode<T> otherUnsafeNode && this.Equals(otherUnsafeNode);
    }

    public bool Equals(UnsafeDisposableDataNode<T> other) {
      // If parameter is null, return false.
      if (other is null) {
        return false;
      }

      // Optimization for a common success case.
      if (Object.ReferenceEquals(this, other)) {
        return true;
      }

      // Return true if the fields match.
      // Note that the base class is not invoked because it is
      // System.Object, which defines Equals as reference equality.
      return this.Data?.Equals(other.Data) ?? false;
    }

    public override int GetHashCode() {
      return this.Data?.GetHashCode() ?? 0;
    }

    public static bool operator ==(UnsafeDisposableDataNode<T> lhs, UnsafeDisposableDataNode<T> rhs) {
      // Check for null on left side.
      if (lhs is null) {
        if (rhs is null) {
          // null == null = true.
          return true;
        }

        // Only the left side is null.
        return false;
      }
      // Equals handles case of null on right side.
      return lhs.Data?.Equals(rhs.Data) ?? false;
    }

    public static bool operator !=(
      UnsafeDisposableDataNode<T> lhs,
      UnsafeDisposableDataNode<T> rhs) {
      return !(lhs == rhs);
    }
  }
}