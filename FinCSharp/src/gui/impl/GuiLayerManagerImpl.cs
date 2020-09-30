using System.Collections.Generic;
using System.Linq;

using fin.app;
using fin.app.events;
using fin.graphics.camera;
using fin.input;
using fin.input.button;

namespace fin.gui.impl {
  // TODO: Calculate reflows. Ideally before inputs are processed.
  public class GuiLayerManagerImpl {
    private IList<IGuiWindow> windows_ = new List<IGuiWindow>();

    private IGuiWindow? activeWindow_;

    private IGuiWindow? windowUnderMouse_;
    private IGuiNode? nodeUnderMouse_;

    [OnTick]
    private void ProcessInputs_(ProcessInputsEvent evt) {
      ICursor cursor = evt.Input.Cursor;

      var cursorPosition = cursor.Position;

      if (cursorPosition == null) {
        this.windowUnderMouse_ = null;
        this.nodeUnderMouse_ = null;
      }
      else {
        var cursorX = cursorPosition.X;
        var cursorY = cursorPosition.Y;

        this.windowUnderMouse_ = this.FindWindowAt_(cursorX, cursorY);
        this.nodeUnderMouse_ = this.FindNodeUnderMouse_(cursorX, cursorY);

        if (cursor.LeftButton.State == ButtonState.PRESSED) {
          this.activeWindow_ = this.windowUnderMouse_;

          this.windows_.Remove(this.activeWindow_!);
          this.windows_.Insert(0, this.activeWindow_!);
        }
      }

      // TODO: How to trigger mouse enter/leave etc. events?
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      var g = evt.Graphics;
      // Must be rendered in reverse order to layer things correctly.
      for (var i = this.windows_.Count - 1; i >= 0; --i) {
        this.windows_[i].Render(g);
      }
    }

    private IGuiWindow? FindWindowAt_(float x, float y)
      => this.windows_.FirstOrDefault(
          window => window.Bounds.Contains(x, y));

    // TODO: This assumes that no nodes are overlapping...
    private IGuiNode? FindNodeUnderMouse_(float x, float y) {
      var children = this.windowUnderMouse_?.Children;
      if (children != null) {
        foreach (var child in children) {
          var result = this.FindNodeUnderMouseInNode_(child, x, y);
          if (result != null) {
            return result;
          }
        }
      }

      return null;
    }

    private IGuiNode? FindNodeUnderMouseInNode_(
        IGuiNode node,
        float x,
        float y) {
      if (!node.Bounds.Contains(x, y)) {
        return null;
      }

      var children = node.Children;
      if (children != null) {
        foreach (var child in children) {
          var result = this.FindNodeUnderMouseInNode_(child, x, y);
          if (result != null) {
            return result;
          }
        }
      }

      return node;
    }
  }
}