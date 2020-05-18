namespace fin.data.collections.grid {
  public interface IGridNode<T> {
    int X { get; }
    int Y { get; }
    T Value { get; set; }
  }

  public interface IReadableFinGrid<T> : IReadableFinCollection<IGridNode<T>> {
    int Width { get; }
    int Height { get; }

    T this[int x, int y] { get; }
  }

  public interface IWritableFinGrid<T> : IWritableFinCollection<IGridNode<T>> {
    T this[int x, int y] { set; }
  }

  public interface IFinGrid<T> : IReadableFinGrid<T>, IWritableFinGrid<T> {
    new T this[int x, int y] { get; set; }
  }
}