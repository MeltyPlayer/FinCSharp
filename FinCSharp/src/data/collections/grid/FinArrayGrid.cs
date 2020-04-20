namespace fin.data.collections.grid {
  using System.Collections.Generic;

  using exception;

  // TODO: Add tests.
  public class FinArrayGrid<T> : IFinGrid<T> {
    private T[] impl_;
    private bool touched = false;
    public int Width { get; }
    public int Height { get; }

    public FinArrayGrid(int width, int height) {
      this.Width = width;
      this.Height = height;
      this.impl_ = new T[width * height];
    }

    public int Count => this.impl_.Length;

    public IEnumerator<T> GetEnumerator() =>
        new ConvertedEnumerator<T>(this.impl_.GetEnumerator(),
                                   obj => (T) obj);

    public bool Clear() {
      if (!this.touched) {
        return false;
      }

      this.impl_ = new T[this.Width * this.Height];
      this.touched = false;
      return true;
    }

    public T this[int x, int y] {
      get => this.impl_[this.CalculateIndex_(x, y)];
      set {
        this.impl_[this.CalculateIndex_(x, y)] = value;
        this.touched = true;
      }
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