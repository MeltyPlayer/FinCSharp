using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

    public T this[int x, int y] {
      get {
        if (this.VerifyIndex_(x, y)) {
          var node = this.GetNodeAtIndex_(x, y, false);
          if (node != null) {
            return node.Value;
          }
        }
        return this.defaultValue_;
      }
      set {
        if (this.VerifyIndex_(x, y)) {
          var node = this.GetNodeAtIndex_(x, y, true);
          node!.Value = value;
        }
      }
    }

    private GridNode? GetNodeAtIndex_(int x, int y, bool shouldCreate) {
      if (this.VerifyIndex_(x, y)) {
        GridNode node;
        if (shouldCreate) {
          return this.impl_.GetOrAdd((x, y),
                                     position => {
                                       var (x, y) = position;
                                       return new GridNode(
                                           x,
                                           y,
                                           this.defaultValue_);
                                     });
        }

        if (this.impl_.TryGet((x, y), out node)) {
          return node;
        }
      }

      return null;
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