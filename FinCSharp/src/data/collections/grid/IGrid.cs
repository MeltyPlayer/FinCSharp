namespace fin.data.collections.grid {

  // TODO: Inherit from ICollection
  public interface IGrid<T> {
    int Width { get; }
    int Height { get; }

    T this[int x, int y] { get; set; }
  }
}