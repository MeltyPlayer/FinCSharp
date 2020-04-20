namespace fin.data.collections.list {
  public interface IReadableFinList<T> : IReadableFinCollection<T> {
    T this[int index] { get; }
  }

  public interface IWritableFinList<T> : IWritableFinCollection<T> {
    T this[int index] { set; }
  }

  public interface IFinList<T> : IReadableFinList<T>, IWritableFinList<T> {
    new T this[int index] { get; set; }
  }
}