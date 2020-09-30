namespace fin.collision {
  // TODO: Should Contains() be templated to any numeric type?
  public interface IBounds2d {
    bool Contains(float x, float y);
  }

  public interface IRectangularBounds2d : IBounds2d {
    float LeftX { get; }
    float TopY { get; }
    float Width { get; }
    float Height { get; }
    
    float RightX { get; }
    float BottomY { get; }
  }
}
