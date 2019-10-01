namespace fin.app {

  public interface IWindow {
    string Title { get; set; }

    int Width { get; set; }
    int Height { get; set; }

    void Close();
  }
}