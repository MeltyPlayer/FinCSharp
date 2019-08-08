using fin.exception;

namespace fin.data.grid {
  public class ArrayGrid<T> : IGrid<T> {
    private readonly T[] data_;
    public int width { get; }
    public int height { get; }

    public ArrayGrid(int width, int height) {
      this.width = width;
      this.height = height;
      data_ = new T[width * height];
    }

    public T this[int x, int y] {
      get { return data_[CalculateIndex_(x, y)]; }
      set { data_[CalculateIndex_(x, y)] = value; }
    }

    private int CalculateIndex_(int x, int y) {
      if (x < 0 || x >= width || y < 0 || y >= height) {
        throw new InvalidIndexException("Invalid position accessed in grid: (" + x + ", " + y + ")");
      }

      return y * width + x;
    }
  }
}
