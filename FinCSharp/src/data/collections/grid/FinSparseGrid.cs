using System.Collections.Generic;

using fin.data.collections.map;
using fin.exception;

namespace fin.data.collections.grid {
  public class FinSparseGrid<T> : IFinGrid<T> {
    public int Width { get; }
    public int Height { get; }

    private readonly T defaultValue_;
    private readonly GridNode defaultNode_;

    private readonly IFinMap<(int, int), GridNode> impl_;

    public bool ShouldThrowExceptions { get; set; } = false;

    public FinSparseGrid(int width, int height, T defaultValue) {
      this.Width = width;
      this.Height = height;

      this.defaultValue_ = defaultValue;
      this.defaultNode_ = new GridNode(0, 0, this.defaultValue_);

      var comparer = new Comparer(width, height);
      this.impl_ = new FinBinaryMap<(int, int), GridNode>(comparer);
    }

    public int Count => this.impl_.Count;

    public IEnumerator<IGridNode<T>> GetEnumerator() =>
        this.impl_.Values.GetEnumerator();

    public bool Clear() => this.impl_.Clear();

    public T this[int c, int r] {
      get {
        if (this.VerifyIndex_(c, r)) {
          var node = this.GetNodeAtIndex_(c, r, false);
          if (node != null) {
            return node.Value;
          }
        }
        return this.defaultValue_;
      }
      set {
        if (this.VerifyIndex_(c, r)) {
          var node = this.GetNodeAtIndex_(c, r, true);
          node!.Value = value;
        }
      }
    }

    private GridNode? GetNodeAtIndex_(int c, int r, bool shouldCreate) {
      if (this.VerifyIndex_(c, r)) {
        GridNode node;
        if (shouldCreate) {
          return this.impl_.GetOrAdd((c, r),
                                     position => {
                                       var (c, r) = position;
                                       return new GridNode(
                                           c,
                                           r,
                                           this.defaultValue_);
                                     });
        }

        if (this.impl_.TryGet((c, r), out node)) {
          return node;
        }
      }

      return null;
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

    private class Comparer : IComparer<(int, int)> {
      private int width_;
      private int height_;

      public Comparer(int width, int height) {
        this.width_ = width;
        this.height_ = height;
      }

      public int Compare((int, int) lhs, (int, int) rhs) {
        var (lhsX, lhsY) = lhs;
        var (rhsX, rhsY) = rhs;

        var lhsI = lhsY * this.width_ + lhsX;
        var rhsI = rhsY * this.width_ + rhsX;

        return lhsI - rhsI;
      }
    }
  }
}