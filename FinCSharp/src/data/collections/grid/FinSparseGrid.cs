using System.Collections.Generic;

using fin.data.collections.map;
using fin.exception;

namespace fin.data.collections.grid {
  public class FinSparseGrid<T> : IFinGrid<T> {
    private readonly IFinMap<(int, int), T> impl_ =
        new FinHashMap<(int, int), T>();

    private readonly T defaultValue_;
    public int Width { get; }
    public int Height { get; }
    
    public FinSparseGrid(int width, int height, T defaultValue) {
      this.Width = width;
      this.Height = height;
      this.defaultValue_ = defaultValue;
    }

    public int Count => this.impl_.Count;
    public IEnumerator<T> GetEnumerator() => this.impl_.Values.GetEnumerator();

    public bool Clear() => this.impl_.Clear();

    public T this[int x, int y] {
      get => this.impl_.GetOrDefault(this.VerifyIndex_(x, y),
                                     this.defaultValue_);
      set => this.impl_[this.VerifyIndex_(x, y)] = value;
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