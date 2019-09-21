using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.app {
  public abstract class ISceneNode {
    private ISceneNode parent_;
    private readonly IList<ISceneNode> children_ = new List<ISceneNode>();

    /**
     * Child actor logic.
     */
    // TODO: Make this only accessible from Scenes to prevent spaghetti code.
    protected ISceneNode GetParent() => this.parent_;
    protected IList<ISceneNode> GetChildren() => this.children_;

    protected ISceneNode Append(params ISceneNode[] children) {
      foreach (var child in children) {
        child.AppendTo(this);
      }

      return this;
    }

    protected ISceneNode AppendTo(ISceneNode parent) {
      if (this.parent_ != parent) {
        this.Remove();

        this.parent_ = parent;
        parent.children_.Add(this);
      }

      return this;
    }

    protected ISceneNode Remove() {
      if (this.parent_ != null) {
        this.parent_.children_.Remove(this);
        this.parent_ = null;
      }

      return this;
    }
  }
}
