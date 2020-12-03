using System.Collections.Generic;
using System.Linq;

using fin.data.graph;
using fin.discardable;
using fin.math;
using fin.math.geometry;

namespace fin.skeleton {
  public interface IBone2 {
    IBone2? Parent { get; }
    IBoneTransform2 Transform { get; }

    IEnumerable<IBone2> Children { get; }
    IBone2 AddChild(float length, float relativeDeg);

    IVector2<float> Start { get; }
    IVector2<float> End { get; }

    // TODO: Remove this.
    void UpdateMatrices();
  }

  // TODO: Support min/max angles.
  public interface IBoneTransform2 {
    float X { get; set; }
    float Y { get; set; }

    float Length { get; set; }

    float RelativeDeg { get; set; }
    float GlobalDeg { get; }
  }


  // Impl

  public sealed class Bone2 : IBone2 {
    private readonly Bone2? parent_;
    private readonly ISet<IBone2> children_ = new HashSet<IBone2>();

    private readonly MutableVector2<float> startImpl_ =
        new MutableVector2<float>();

    private readonly MutableVector2<float> endImpl_ =
        new MutableVector2<float>();

    private Bone2(Bone2? parent, float length) {
      this.parent_ = parent;

      this.transform_ = new BoneTransform2(length);
    }

    public static IBone2 NewRoot(float relativeDeg) {
      var root = new Bone2(null, 0);
      root.Transform.RelativeDeg = relativeDeg;
      return root;
    }

    public IBone2? Parent => this.parent_;

    public IBoneTransform2 Transform => this.transform_;
    private readonly BoneTransform2 transform_;

    public IEnumerable<IBone2> Children => this.children_;

    public IBone2 AddChild(
        float length,
        float relativeDeg) {
      var child = new Bone2(this, length);
      child.Transform.RelativeDeg = relativeDeg;

      this.children_.Add(child);

      return child;
    }

    public IVector2<float> Start => this.startImpl_;
    public IVector2<float> End => this.endImpl_;

    // TODO: Remove this.
    public void UpdateMatrices() {
      if (this.parent_ == null) {
        this.endImpl_.X = this.startImpl_.X = this.Transform.X;
        this.endImpl_.Y = this.startImpl_.Y = this.Transform.Y;

        this.transform_.GlobalDeg = this.Transform.RelativeDeg;
        return;
      }

      var startX = this.startImpl_.X = this.parent_.End.X + this.Transform.X;
      var startY = this.startImpl_.Y = this.parent_.End.Y + this.Transform.Y;

      var length = this.transform_.Length;
      var globDeg = this.transform_.GlobalDeg =
                        this.parent_.transform_.GlobalDeg +
                        this.transform_.RelativeDeg;

      this.endImpl_.X = startX + TrigMath.LenDegX(length, globDeg);
      this.endImpl_.Y = startY - TrigMath.LenDegY(length, globDeg);
    }

    // TODO: Update matrix when this is changed?
    private class BoneTransform2 : IBoneTransform2 {
      public BoneTransform2(float length) {
        this.Length = length;
      }

      public float X { get; set; }
      public float Y { get; set; }

      public float Length { get; set; }

      public float RelativeDeg { get; set; }
      public float GlobalDeg { get; set; }
    }
  }
}