using System;

using fin.assert;

namespace fin.math {
  public sealed class FloatMath : IMath<float> {
    public static readonly FloatMath INSTANCE = new FloatMath();

    private FloatMath() {}

    // Operations
    public static float Mod(float lhs, float rhs)
      => FloatMath.INSTANCE._Mod(lhs, rhs);

    public static float Sqrt(float value) => FloatMath.INSTANCE._Sqrt(value);

    public static float Pow(float value, float n)
      => FloatMath.INSTANCE._Pow(value, n);

    public float _Mod(float lhs, float rhs) => lhs % rhs;
    public float _Sqrt(float value) => (float) Math.Sqrt(value);

    public float _Pow(float value, float exponent)
      => (float) Math.Pow(value, exponent);

    // Working w/ averages

    // Working w/ signs
    public static float Abs(float value) => FloatMath.INSTANCE._Abs(value);
    public static float Sign(float value) => FloatMath.INSTANCE._Sign(value);
    public static bool IsPositive(float v) => FloatMath.INSTANCE._IsPositive(v);
    public static bool IsNegative(float v) => FloatMath.INSTANCE._IsNegative(v);

    public float _Abs(float v) => v >= 0 ? v : -v;
    public int _Sign(float v) => v > 0 ? 1 : v < 0 ? -1 : 0;
    public bool _IsPositive(float value) => value > 0;
    public bool _IsNegative(float value) => value < 0;

    // Checking number ranges
    public static bool IsBetween(float min, float value, float max)
      => FloatMath.INSTANCE._IsBetween(min, value, max);

    public bool _IsBetween(float min, float value, float max) {
      Asserts.True(min <= max, "Expected min <= max.");
      return min <= value && value <= max;
    }

    /* public static bool IsIncreasing<TNumber>(params TNumber[] values)
        where TNumber : IComparable {
      var previousValue = values[0];
      foreach (var value in values) {
        if (FinMath.IsGreaterThan(previousValue, value)) {
          return false;
        }
        previousValue = value;
      }
      return true;
    }

    public static bool IsDecreasing<TNumber>(params TNumber[] values)
        where TNumber : IComparable {
      var previousValue = values[0];
      foreach (var value in values) {
        if (FinMath.IsLessThan(previousValue, value)) {
          return false;
        }
        previousValue = value;
      }
      return true;
    }*/

    // Applying value ranges
    public static float Min(float lhs, float rhs)
      => FloatMath.INSTANCE._Min(lhs, rhs);

    public static float Max(float lhs, float rhs)
      => FloatMath.INSTANCE._Max(lhs, rhs);

    public static float Clamp(float min, float value, float max)
      => FloatMath.INSTANCE._Clamp(min, value, max);

    public static float Wrap(float min, float value, float max)
      => FloatMath.INSTANCE._Wrap(min, value, max);

    public static float AddTowards(float start, float end, float inc)
      => FloatMath.INSTANCE._AddTowards(start, end, inc);


    public float _Min(float lhs, float rhs) => lhs < rhs ? lhs : rhs;
    public float _Max(float lhs, float rhs) => lhs > rhs ? lhs : rhs;

    public float _Clamp(float min, float value, float max) {
      Asserts.True(min <= max, "Expected min <= max.");
      return FloatMath.Max(min, FloatMath.Min(value, max));
    }

    public float _Wrap(float min, float value, float max) {
      Asserts.True(min <= max, "Expected min <= max.");

      var range = max - min;

      var firstPass = max - FloatMath.Mod(max - value, range);
      var secondPass = min + FloatMath.Mod(firstPass - min, range);

      return secondPass;
    }

    public float _AddTowards(float start, float end, float inc) {
      Asserts.True(inc >= 0, "Expected increment to be positive.");

      var range = end - start;

      if (FloatMath.Abs(range) <= inc) {
        return end;
      }

      var sign = FloatMath.Sign(range);
      return start + sign * inc;
    }
  }
}