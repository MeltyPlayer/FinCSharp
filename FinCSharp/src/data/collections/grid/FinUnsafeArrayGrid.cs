using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using fin.data.collections.list;
using fin.exception;

namespace fin.data.collections.grid {
  // TODO: Add tests.
  public class FinUnsafeArrayGrid<T> : IFinGrid<T> {
    private readonly T[] impl_;

    public int Width { get; }
    public int Height { get; }

    private readonly T defaultValue_;

    public FinUnsafeArrayGrid(int width, int height, T defaultValue) {
      this.Width = width;
      this.Height = height;

      this.defaultValue_ = defaultValue;

      var size = width * height;
      this.impl_ = new T[size];
      for (var r = 0; r < height; ++r) {
        for (var c = 0; c < width; ++c) {
          var i = this.CalculateIndex_(c, r);
          this.impl_[i] = defaultValue;
        }
      }
    }

    public int Count => this.impl_.Length;

    public IEnumerator<IGridNode<T>> GetEnumerator()
      => throw new NotImplementedException();

    public bool Clear() => throw new NotImplementedException();

    public T this[int c, int r] {
      get => this.impl_[this.CalculateIndex_(c, r)];
      set => this.impl_[this.CalculateIndex_(c, r)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateIndex_(int c, int r) => r * this.Width + c;
  }
}