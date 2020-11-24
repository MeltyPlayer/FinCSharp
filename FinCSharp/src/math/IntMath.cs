using System;

using fin.assert;

namespace fin.math {
  public sealed class IntMath : IMath<int> {
    public static readonly IntMath INSTANCE = new IntMath();

    private IntMath() {}

    // Operations
    public static int Mod(int lhs, int rhs) => IntMath.INSTANCE._Mod(lhs, rhs);
    public static int Sqrt(int value) => IntMath.INSTANCE._Sqrt(value);
    public static int Pow(int value, int n) => IntMath.INSTANCE._Pow(value, n);

    public int _Mod(int lhs, int rhs) => lhs % rhs;
    public int _Sqrt(int value) => (int) Math.Sqrt(value);
    public int _Pow(int value, int exponent) => (int) Math.Pow(value, exponent);

    // Working w/ averages

    // Working w/ signs
    public static int Abs(int value) => IntMath.INSTANCE._Abs(value);
    public static int Sign(int value) => IntMath.INSTANCE._Sign(value);
    public static bool IsPositive(int v) => IntMath.INSTANCE._IsPositive(v);
    public static bool IsNegative(int v) => IntMath.INSTANCE._IsNegative(v);

    public int _Abs(int v) => v >= 0 ? v : -v;
    public int _Sign(int v) => v > 0 ? 1 : v < 0 ? -1 : 0;
    public bool _IsPositive(int value) => value > 0;
    public bool _IsNegative(int value) => value < 0;

    // Checking number ranges
    public static bool IsBetween(int min, int value, int max)
      => IntMath.INSTANCE._IsBetween(min, value, max);

    public bool _IsBetween(int min, int value, int max) {
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
    public static int Min(int lhs, int rhs) => IntMath.INSTANCE._Min(lhs, rhs);
    public static int Max(int lhs, int rhs) => IntMath.INSTANCE._Max(lhs, rhs);

    public static int Clamp(int min, int value, int max)
      => IntMath.INSTANCE._Clamp(min, value, max);

    public static int Wrap(int min, int value, int max)
      => IntMath.INSTANCE._Wrap(min, value, max);

    public static int AddTowards(int start, int end, int inc)
      => IntMath.INSTANCE._Wrap(start, end, inc);


    public int _Min(int lhs, int rhs) => lhs < rhs ? lhs : rhs;
    public int _Max(int lhs, int rhs) => lhs > rhs ? lhs : rhs;

    public int _Clamp(int min, int value, int max) {
      Asserts.True(min <= max, "Expected min <= max.");
      return IntMath.Max(min, IntMath.Min(value, max));
    }

    public int _Wrap(int min, int value, int max) {
      Asserts.True(min <= max, "Expected min <= max.");

      var range = max - min;

      var firstPass = max - IntMath.Mod(max - value, range);
      var secondPass = min + IntMath.Mod(firstPass - min, range);

      return secondPass;
    }

    public int _AddTowards(int start, int end, int inc) {
      Asserts.True(inc > 0, "Expected increment to be positive.");

      var range = end - start;

      if (IntMath.Abs(range) < inc) {
        return end;
      }

      var sign = IntMath.Sign(range);
      return start + sign * inc;
    }
  }
}