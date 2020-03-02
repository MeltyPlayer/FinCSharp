namespace fin.data.collections.grid {
  using exception;

  // TODO: Add tests.
  public class ArrayGrid<T> : IGrid<T> {
    private readonly T[] data_;
    public int Width { get; }
    public int Height { get; }

    public ArrayGrid(int width, int height) {
      this.Width = width;
      this.Height = height;
      this.data_ = new T[width * height];
    }

    public T this[int x, int y] {
      get { return this.data_[this.CalculateIndex_(x, y)]; }
      set { this.data_[this.CalculateIndex_(x, y)] = value; }
    }

    private int CalculateIndex_(int x, int y) {
      if (x < 0 || x >= this.Width || y < 0 || y >= this.Height) {
        throw new InvalidIndexException("Invalid position accessed in grid: (" +
                                        x + ", " + y + ")");
      }

      return y * this.Width + x;
    }
  }
}