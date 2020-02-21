using System;
using System.Collections.Generic;
using fin.log;

namespace fin.math.geometry {

  public interface INestedBoundingBoxes<TNumber> : IBoundingBox<TNumber> where TNumber : IComparable {
    int Depth { get; }

    void Push(IBoundingBox<TNumber> box);
    void Pop();

    IBoundingBox<TNumber>? DetermineIntersection();
  }

  public class NestedBoundingBoxes<TNumber> : INestedBoundingBoxes<TNumber> where TNumber : IComparable {
    private readonly Stack<IBoundingBox<TNumber>> impl_ = new Stack<IBoundingBox<TNumber>>();

    public IVector2<TNumber> TopLeft => this.DetermineIntersection()!.TopLeft;
    public IDimensions<TNumber> Dimensions => this.DetermineIntersection()!.Dimensions;

    public int Depth => this.impl_.Count;
    public void Push(IBoundingBox<TNumber> boundingBox) => this.impl_.Push(boundingBox);
    public void Pop() => this.impl_.Pop();

    public IBoundingBox<TNumber>? DetermineIntersection() {
      if (this.Depth == 0) {
        return null;
      }

      MutableBoundingBox<TNumber>? intersection = null;
      foreach (var current in this.impl_) {
        var currentTopLeft = current.TopLeft;
        var currentDimensions = current.Dimensions;

        var currentLeftX = currentTopLeft.X;
        var currentTopY = currentTopLeft.Y;
        var currentWidth = currentDimensions.Width;
        var currentHeight = currentDimensions.Height;

        if (intersection == null) {
          intersection = new MutableBoundingBox<TNumber>(currentLeftX, currentTopY, currentWidth, currentHeight);
        } else {
          // TODO: Figure out how to fix this...
          var currentRightX = currentLeftX + (dynamic)currentDimensions.Width;
          var currentBottomY = currentTopY + (dynamic)currentDimensions.Height;

          var boxTopLeft = intersection.TopLeft;
          var boxDimensions = intersection.Dimensions;
          var boxLeftX = boxTopLeft.X;
          var boxTopY = boxTopLeft.Y;
          var boxWidth = boxDimensions.Width;
          var boxHeight = boxDimensions.Height;
          var boxRightX = boxLeftX + (dynamic)boxWidth;
          var boxBottomY = boxTopY + (dynamic)boxHeight;

          var adjustedLeftX = Max_(currentLeftX, boxLeftX);
          var adjustedRightX = Min_(currentRightX, boxRightX);
          var adjustedTopY = Max_(currentTopY, boxTopY);
          var adjustedBottomY = Min_(currentBottomY, boxBottomY);
          var adjustedWidth = Math.Max(0, adjustedRightX - adjustedLeftX);
          var adjustedHeight = Math.Max(0, adjustedBottomY - adjustedTopY);

          intersection.TopLeft.X = adjustedLeftX;
          intersection.TopLeft.Y = adjustedTopY;
          intersection.Dimensions.Width = adjustedWidth;
          intersection.Dimensions.Height = adjustedHeight;
        }
      }
      return intersection;
    }

    private static TComparable Max_<TComparable>(TComparable left, TComparable right) where TComparable : IComparable
      => left.CompareTo(right) > 0 ? left : right;
    private static TComparable Min_<TComparable>(TComparable left, TComparable right) where TComparable : IComparable
      => left.CompareTo(right) < 0 ? left : right;
  }
}