using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.data {
  public class InvalidIndexException : Exception {
    public InvalidIndexException() { }

    public InvalidIndexException(string message) : base(message) { }

    public InvalidIndexException(string message, Exception inner) : base(message, inner) { }
  }

  public interface IGrid<T> {
    int width { get; }
    int height { get; }

    T this[int x, int y] { get; set; }
  }

  public class Grid<T> : IGrid<T> {
    private readonly T[] data_;
    public int width { get; }
    public int height { get; }

    public Grid(int width, int height) {
      this.width = width;
      this.height = height;
      data_ = new T[width * height];
    }

    public T this[int x, int y] {
      get { return data_[CalculateIndex_(x, y)]; }
      set { data_[CalculateIndex_(x, y)] = value; }
    }

    private int CalculateIndex_(int x, int y) {
      if (x < 0 || x >= width || y < 0 || y >= height) {
        throw new InvalidIndexException("Invalid position accessed in grid: (" + x + ", " + y + ")");
      }

      return y * width + x;
    }
  }
}
