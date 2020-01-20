using System.Linq;
using fin.assert;
using fin.data.tree;

namespace fin.app {

  public class ResourceNode {
    private readonly Node<ResourceNode> node_;

    public delegate void OnDestroyHandler();

    public event OnDestroyHandler OnDestroy = delegate { };

    public ResourceNode() {
      this.node_ = new Node<ResourceNode>(this);
    }

    protected bool IsDestroyed => this.node_.FromNodes.Count() == 0;

    protected void RemoveFromParentAndDestroy() {
      Asserts.False(this.IsDestroyed);

      this.node_.RemoveFrom(this.Parent().node_);
      this.OnDestroy();

      foreach (var childNode in this.node_.ToNodes) {
        childNode.Value.RemoveFromParentAndDestroy();
      }
    }

    protected ResourceNode Parent() => this.node_.FromNodes.First().Value;

    public void AddChild(ResourceNode other) {
      this.node_.AddTo(other.node_);
    }
  }
}