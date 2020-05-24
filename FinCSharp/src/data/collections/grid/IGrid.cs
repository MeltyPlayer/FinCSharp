namespace fin.data.collections.grid {
  public interface IGridNode<T> {
    int C { get; }
    int R { get; }
    T Value { get; set; }
  }

  public interface IReadableFinGrid<T> : IReadableFinCollection<IGridNode<T>> {
    int Width { get; }
    int Height { get; }

    T this[int c, int r] { get; }
  }

  public interface IWritableFinGrid<T> : IWritableFinCollection<IGridNode<T>> {
    T this[int c, int r] { set; }
  }

  public interface IFinGrid<T> : IReadableFinGrid<T>, IWritableFinGrid<T> {
    new T this[int c, int r] { get; set; }
  }
}