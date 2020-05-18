using System.Collections.Generic;

using fin.data.collections.list;
using fin.exception;

namespace fin.data.collections.grid {
  // TODO: Add tests.
  public class FinArrayGrid<T> : IFinGrid<T> {
    private IFinList<GridNode> impl_;
    private bool touched_ = false;

    public int Width { get; }
    public int Height { get; }

    private readonly T defaultValue_;

    public bool ShouldThrowExceptions { get; set; } = true;

    public FinArrayGrid(int width, int height, T defaultValue) {
      this.Width = width;
      this.Height = height;

      this.defaultValue_ = defaultValue;

      var size = width * height;
      this.impl_ = new FinArrayList<GridNode>(size);
      for (var y = 0; y < height; ++y) {
        for (var x = 0; x < width; ++x) {
          var i = this.CalculateIndex_(x, y);
          this.impl_[i] = new GridNode(x, y, defaultValue);
        }
      }
    }

    public int Count => this.impl_.Count;

    public IEnumerator<IGridNode<T>> GetEnumerator() =>
        this.impl_.GetEnumerator();

    public bool Clear() {
      if (!this.touched_) {
        return false;
      }

      foreach (var node in this.impl_) {
        node.Value = this.defaultValue_;
      }

      this.touched_ = false;
      return true;
    }

    public T this[int x, int y] {
      get => this.VerifyIndex_(x, y)
                 ? this.impl_[this.CalculateIndex_(x, y)].Value
                 : this.defaultValue_;
      set {
        if (this.VerifyIndex_(x, y)) {
          this.impl_[this.CalculateIndex_(x, y)].Value = value;
          this.touched_ = true;
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

    private class GridNode : IGridNode<T> {
      public int X { get; }
      public int Y { get; }
      public T Value { get; set; }

      public GridNode(int x, int y, T value) {
        this.X = x;
        this.Y = y;
        this.Value = value;
      }
    }
  }
}