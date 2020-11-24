/*using System;
using System.Drawing;

using fin.graphics;
using fin.graphics.color;

namespace fin.gui.impl {
  public class GuiFactory : IGuiFactory {
    public IContentFactory Content { get; } = new ContentFactory();
    public class ContentFactory : IContentFactory {
      public IPanel NewPanel(ILayout layout);

      public sealed class Panel : IPanel {}


      public IButton NewButton(Action handler) => new Button(handler);

      public sealed class Button : IButton {
        private readonly Action handler_;

        public Button(Action handler) {
          this.handler_ = handler;
        }

        public void Click(int x, int y) => this.handler_();

        // TODO: Rewrite this in terms of static primitives.
        public void Render(IGraphics g, int x, int y, int width, int height) {
          g.Primitives.VertexColor(ColorConstants.CYAN);
          g.Render2d.Rectangle(x, y, width, height, true);
        }
      }
    }
  }
}*/