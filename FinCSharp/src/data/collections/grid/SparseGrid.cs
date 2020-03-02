namespace fin.data.collections.grid {
  using System.Collections.Generic;

  using exception;

  public class SparseGrid<T> : IGrid<T> {
    private readonly IDictionary<(int, int), T> data_ =
        new Dictionary<(int, int), T>();

    private readonly T defaultValue_;
    public int Width { get; }
    public int Height { get; }

    public SparseGrid(int width, int height, T defaultValue) {
      this.Width = width;
      this.Height = height;
      this.defaultValue_ = defaultValue;
    }

    public T this[int x, int y] {
      get => this.data_.TryGetValue(this.VerifyIndex_(x, y), out var value)
                 ? value
                 : this.defaultValue_;
      set => this.data_[this.VerifyIndex_(x, y)] = value;
    }

    private (int, int) VerifyIndex_(int x, int y) {
      if (x < 0 || x >= this.Width || y < 0 || y >= this.Height) {
        throw new InvalidIndexException("Invalid position accessed in grid: (" +
                                        x + ", " + y + ")");
      }

      return (x, y);
    }
  }
}