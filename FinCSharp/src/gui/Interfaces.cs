using fin.collision;
using fin.data.collections.list;
using fin.graphics;

namespace fin.gui {
  public interface IGuiElement {
    IBounds2d Bounds { get; }

    IGuiNode? Parent { get; }
    IReadableFinList<IGuiNode>? Children { get; }

    void Render(IGraphics g);
  }

  public interface IGuiNode : IGuiElement {
  }

  public interface IGuiLeafNode : IGuiNode {
    IReadableFinList<IGuiNode>? IGuiElement.Children => null;
  }
  public interface IGuiParentNode : IGuiNode {
  }

  public interface IGuiWindow : IGuiParentNode {
    IGuiNode? IGuiElement.Parent => null;
  }
}
