namespace fin.data.collections.set {
  public interface IFinSet<T> : IReadableFinSet<T>, IWritableFinSet<T> {}

  public interface IReadableFinSet<T> : IReadableFinCollection<T> {
    bool Contains(T value);
  }

  public interface IWritableFinSet<T> : IWritableFinCollection<T> {
    bool Add(T value);
    bool Remove(T value);
  }
}