using fin.app.node;
using fin.graphics.common;
using fin.math.geometry;

namespace fin.app.window {

  public interface IWindow : IComponent {
    string Title { get; set; }

    int Width { get; set; }
    int Height { get; set; }

    bool Visible { get; set; }

    IView NewView(IBoundingBox<int> boundingBox);

    void Close();
  }
}