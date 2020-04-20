namespace fin.data.collections.grid {
  // TODO: Inherit from ICollection
  public interface IReadableFinGrid<T> : IReadableFinCollection<T> {
    int Width { get; }
    int Height { get; }

    T this[int x, int y] { get; }
  }

  public interface IWritableFinGrid<T> : IWritableFinCollection<T> {
    T this[int x, int y] { set; }
  }

  public interface IFinGrid<T>: IReadableFinGrid<T>, IWritableFinGrid<T> {
    new T this[int x, int y] { get; set; }
  }
}