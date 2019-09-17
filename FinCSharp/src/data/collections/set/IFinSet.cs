namespace fin.data.collections.set {
  public interface IFinSet<T> : IFinCollection<T> {
    bool Add(T instance);
    bool Remove(T instance);
  }
}