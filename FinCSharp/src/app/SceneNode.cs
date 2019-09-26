using System.Collections.Generic;

using fin.app.phase;
using fin.dispose;

namespace fin.app {

  public abstract class SceneNode : ITickHandler {

    protected delegate void OnDisposeEventHandler();

    protected event OnDisposeEventHandler OnDisposeEvent = delegate { };

    private readonly UnsafeDisposableDataNode<SceneNode> impl_;

    // TODO: Switch out null for the parent based on current scope.
    public SceneNode(SceneNode? parent = null) {
      this.impl_ = new UnsafeDisposableDataNode<SceneNode>(this, parent?.impl_);

      this.impl_.OnDisposeEvent += this.OnDispose_;
    }

    protected void TriggerDispose() => this.impl_.Dispose();

    private void OnDispose_() {
      this.OnDisposeEvent();
    }

    private SceneNode? parent_;
    private readonly IList<SceneNode> children_ = new List<SceneNode>();

    // TODO: Switch out null for the parent based on current scope.
    public SceneNode? Parent => this.impl_.Parent?.Data;

    public ISet<SceneNode> Children => this.impl_.ChildData;

    public SceneNode Attach(params SceneNode[] children) {
      foreach (var child in children) {
        this.impl_.Attach(child.impl_);
      }
      return this;
    }

    private readonly TickHandler tickHandler_ = new TickHandler();

    public void Tick(params object[] phaseDatas) => this.tickHandler_.Tick(phaseDatas);
  }
}