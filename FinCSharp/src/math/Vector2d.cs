namespace fin.math {

  public interface IVector2 {
    public float X { get; }
    public float Y { get; }
  }

  public struct ImmutableVector2 : IVector2 {

    public ImmutableVector2(float x, float y) {
      this.X = x;
      this.Y = y;
    }

    public float X { get; }
    public float Y { get; }
  }
}