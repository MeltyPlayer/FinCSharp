using fin.app.node;

namespace fin.app {

  public interface IWindow : IComponent {
    string Title { get; set; }

    int Width { get; set; }
    int Height { get; set; }

    void Close();
  }
}