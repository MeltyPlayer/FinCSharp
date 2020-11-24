using fin.math;
using fin.math.geometry;


namespace fin.collision {
  public class BoundingBoxBounds2d : IRectangularBounds2d {
    private readonly IBoundingBox<float> boundingBox_;

    public BoundingBoxBounds2d(IBoundingBox<float> boundingBox) {
      this.boundingBox_ = boundingBox;
    }

    public float LeftX => this.boundingBox_.LeftX;
    public float TopY => this.boundingBox_.TopY;
    public float Width => this.boundingBox_.Width;
    public float Height => this.boundingBox_.Height;

    public float RightX => this.boundingBox_.RightX;
    public float BottomY => this.boundingBox_.BottomY;

    public bool Contains(float x, float y) =>
        FloatMath.IsBetween(this.LeftX, x, this.RightX) &&
        FloatMath.IsBetween(this.TopY, y, this.BottomY);
  }
}