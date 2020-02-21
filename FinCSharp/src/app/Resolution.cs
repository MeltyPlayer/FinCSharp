using fin.math.geometry;

namespace fin.app {

  public class Resolution : IDimensions<int> {
    public int Width { get; }
    public int Height { get; }

    public Resolution(int width, int height) {
      this.Width = width;
      this.Height = height;
    }
  }
}