namespace fin.data.collections.grid {
  using System.Collections.Generic;

  using exception;

  // TODO: Add tests.
  public class FinArrayGrid<T> : IFinGrid<T> {
    private T[] impl_;
    private bool touched = false;
    public int Width { get; }
    public int Height { get; }

    private readonly T defaultValue_;

    public bool ShouldThrowExceptions { get; set; } = true;

    public FinArrayGrid(int width, int height, T defaultValue) {
      this.Width = width;
      this.Height = height;

      this.impl_ = new T[width * height];
      this.defaultValue_ = defaultValue;
      for (var i = 0; i < this.impl_.Length; ++i) {
        this.impl_[i] = defaultValue;
      }
    }

    public int Count => this.impl_.Length;

    public IEnumerator<T> GetEnumerator() =>
        new ConvertedEnumerator<T>(this.impl_.GetEnumerator(),
                                   obj => (T) obj);

    public bool Clear() {
      if (!this.touched) {
        return false;
      }

      var size = this.Width * this.Height;
      this.impl_ = new T[size];
      for (var i = 0; i < size; ++i) {
        this.impl_[i] = this.defaultValue_;
      }

      this.touched = false;
      return true;
    }

    public T this[int x, int y] {
      get => this.VerifyIndex_(x, y)
                 ? this.impl_[this.CalculateIndex_(x, y)]
                 : this.defaultValue_;
      set {
        if (this.VerifyIndex_(x, y)) {
          this.impl_[this.CalculateIndex_(x, y)] = value;
          this.touched = true;
        }
      }
    }

    private bool VerifyIndex_(int x, int y) {
      if (x < 0 || x >= this.Width || y < 0 || y >= this.Height) {
        if (this.ShouldThrowExceptions) {
          throw new InvalidIndexException(
              "Invalid position accessed in grid: (" +
              x + ", " + y + ")");
        }
        return false;
      }
      return true;
    }

    private int CalculateIndex_(int x, int y) => y * this.Width + x;
  }
}