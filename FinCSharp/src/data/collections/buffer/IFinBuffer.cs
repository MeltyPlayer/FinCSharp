namespace fin.data.collections.buffer {
  // TODO: Do we need the readable/writable interfaces...?
  // TODO: Do we need to wrap things immutably??
  public interface IFinBuffer<T> : IFinCollection<T>,
                                   IReadableFinBuffer<T>,
                                   IWritableFinBuffer<T> {
  }

  public interface IReadableFinBuffer<T> : IReadableFinCollection<T> {
    T Next { get; }
  }

  public interface IWritableFinBuffer<T> : IWritableFinCollection<T> {}
}