namespace fin.app {
  public interface IWindow {
    int width { get; set; }
    int height { get; set; }

    void Close();
  }
}