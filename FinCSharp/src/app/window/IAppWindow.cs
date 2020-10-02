using fin.app.node;
using fin.graphics;
using fin.math.geometry;

namespace fin.app.window {
  public interface IAppWindow {
    string Title { get; set; }

    int Width { get; set; }
    int Height { get; set; }

    bool Visible { get; set; }

    IView NewView(IBoundingBox<int> boundingBox);

    void Close();
  }
}