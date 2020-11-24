using System;

namespace fin.math {
  public static class TrigMath {
    public static float DistanceBetween(float x1, float y1, float x2, float y2)
      => FloatMath.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

    public static float DegreesBetween(float x1, float y1, float x2, float y2)
      => (float) (Math.Atan2(y2 - y1, x2 - x1) / Math.PI * 180);

    public static float DifferenceInDegrees(float degLhs, float degRhs) {
      return ((((degLhs - degRhs) % 360) + 540) % 360) - 180;
    }

    public static float AddTowardsDegrees(
        float degStart,
        float degEnd,
        float inc) {
      var diff = TrigMath.DifferenceInDegrees(degEnd, degStart);

      if (FloatMath.Abs(diff) < inc) {
        return degEnd;
      }

      return degStart + FloatMath.Sign(diff) * inc;
    }

    public static float LenDegX(float len, float deg)
      => len * MathF.Cos(deg / 180 * MathF.PI);

    public static float LenDegY(float len, float deg)
      => len * MathF.Sin(deg / 180 * MathF.PI);
  }
}