using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using fin.collision;
using fin.data.collections.list;
using fin.graphics;
using fin.gui;

namespace fin.gui.impl {
  /*public class GuiWindowImpl : IGuiWindow {
    public IRectangularBounds2d Bounds { get; }

    public IReadableFinList<IGuiNode>? Children { get; }

    private readonly IFinList<IGuiNode> children_ =
        new FinVectorList<IGuiNode>();

    public GuiWindowImpl(IRectangularBounds2d bounds) {
      this.Bounds = bounds;
      this.Children = new ImmutablyWrappedFinList<IGuiNode>(this.children_);
    }

    public void Render(IGraphics g) {
      g.Render2d.Rectangle(this.Bounds.LeftX, this.Bounds.TopY, this.Bounds.true)

      foreach (var child in this.children_) {
        child.Render(g);
      }
    }
  }*/
}