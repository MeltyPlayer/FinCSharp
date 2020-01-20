using System.Collections.Generic;

namespace fin.discard {

  public class SafeDisposableNode {

    protected delegate void OnDisposeEventHandler();

    protected event OnDisposeEventHandler OnDisposeEvent = delegate { };

    private readonly UnsafeDisposableDataNode<SafeDisposableNode> impl_;

    // TODO: Switch out null for the parent based on current scope.
    public SafeDisposableNode(SafeDisposableNode? parent = null) {
      this.impl_ = new UnsafeDisposableDataNode<SafeDisposableNode>(this,
        parent?.impl_);

      this.impl_.OnDiscardEvent += this.OnDispose_;
    }

    protected void TriggerDispose() => this.impl_.Discard();

    private void OnDispose_() {
      this.OnDisposeEvent();
    }

    // TODO: Switch out null for the parent based on current scope.
    public SafeDisposableNode? Parent => this.impl_.Parent?.Data;

    public ISet<SafeDisposableNode> Children => this.impl_.ChildData;

    public SafeDisposableNode Attach(params SafeDisposableNode[] children) {
      foreach (var child in children) {
        this.impl_.Attach(child.impl_);
      }
      return this;
    }
  }
}