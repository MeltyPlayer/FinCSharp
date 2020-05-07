using System;

using fin.assert;

using CMath = System.Math;

namespace fin.math {
  public static class Math {
    /**
     * Constants
     */
    public const double PI = CMath.PI;

    public const double TAU = 2 * Math.PI;

    /**
     * Operations
     */
    public static TNumber Mod<TNumber>(TNumber a, TNumber b)
        where TNumber : IComparable => (dynamic) a % (dynamic) b;

    /**
     * Checking number ranges
     */
    public static bool IsGreaterThan<TNumber>(TNumber a, TNumber b)
        where TNumber : IComparable
      => a.CompareTo(b) > 0;

    public static bool IsGreaterThanOrEqualTo<TNumber>(TNumber a, TNumber b)
        where TNumber : IComparable
      => a.CompareTo(b) >= 0;

    public static bool IsLessThan<TNumber>(TNumber a, TNumber b)
        where TNumber : IComparable
      => a.CompareTo(b) < 0;

    public static bool IsLessThanOrEqualTo<TNumber>(TNumber a, TNumber b)
        where TNumber : IComparable
      => a.CompareTo(b) <= 0;

    public static bool IsBetween<TNumber>(
        TNumber min,
        TNumber value,
        TNumber max)
        where TNumber : IComparable {
      Asserts.True(Math.IsLessThanOrEqualTo(min, max), "Expected min <= max.");
      return Math.IsGreaterThanOrEqualTo(value, min) &&
             Math.IsLessThanOrEqualTo(value, max);
    }

    /**
     * Applying value ranges
     */
    public static TNumber Min<TNumber>(TNumber a, TNumber b)
        where TNumber : IComparable
      => Math.IsLessThan(a, b) ? a : b;

    public static TNumber Max<TNumber>(TNumber a, TNumber b)
        where TNumber : IComparable
      => Math.IsGreaterThan(a, b) ? a : b;

    public static TNumber Clamp<TNumber>(
        TNumber min,
        TNumber value,
        TNumber max)
        where TNumber : IComparable {
      Asserts.True(Math.IsLessThanOrEqualTo(min, max), "Expected min <= max.");
      return Math.Max(min, Math.Min(value, max));
    }

    public static TNumber Wrap<TNumber>(
        TNumber min,
        TNumber value,
        TNumber max)
        where TNumber : IComparable {
      Asserts.True(Math.IsLessThanOrEqualTo(min, max), "Expected min <= max.");

      dynamic dMin = min;
      dynamic dValue = value;
      dynamic dMax = max;

      var range = dMax - dMin;

      var firstPass = dMax - Math.Mod(dMax - dValue, range);
      var secondPass = dMin + Math.Mod(firstPass - dMin, range);

      return secondPass;
    }
  }
}