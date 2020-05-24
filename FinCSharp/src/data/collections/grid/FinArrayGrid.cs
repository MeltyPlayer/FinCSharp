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
      for (var r = 0; r < height; ++r) {
        for (var c = 0; c < width; ++c) {
          var i = this.CalculateIndex_(c, r);
          this.impl_[i] = new GridNode(c, r, defaultValue);
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

    public T this[int c, int r] {
      get => this.VerifyIndex_(c, r)
                 ? this.impl_[this.CalculateIndex_(c, r)].Value
                 : this.defaultValue_;
      set {
        if (this.VerifyIndex_(c, r)) {
          this.impl_[this.CalculateIndex_(c, r)].Value = value;
          this.touched_ = true;
        }
      }
    }

    private bool VerifyIndex_(int c, int r) {
      if (c < 0 || c >= this.Width || r < 0 || r >= this.Height) {
        if (this.ShouldThrowExceptions) {
          throw new InvalidIndexException(
              "Invalid position accessed in grid: (" +
              c + ", " + r + ")");
        }
        return false;
      }
      return true;
    }

    private int CalculateIndex_(int c, int r) => r * this.Width + c;

    private class GridNode : IGridNode<T> {
      public int C { get; }
      public int R { get; }
      public T Value { get; set; }

      public GridNode(int c, int r, T value) {
        this.C = c;
        this.R = r;
        this.Value = value;
      }
    }
  }
}