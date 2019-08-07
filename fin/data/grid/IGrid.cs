namespace fin.data.grid {
  public interface IGrid<T> {
    int width { get; }
    int height { get; }

    T this[int x, int y] { get; set; }
  }
}
