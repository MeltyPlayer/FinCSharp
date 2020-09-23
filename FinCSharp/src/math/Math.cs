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
    public static TNumber Mod<TNumber>(TNumber lhs, TNumber rhs)
        where TNumber : IComparable => (dynamic) lhs % (dynamic) rhs;

    // TODO: Dear god. Fix this.
    public static TNumber Sqrt<TNumber>(TNumber value)
      => (TNumber) (dynamic) CMath.Sqrt((double) (dynamic) value!);

    public static TNumber Pow<TNumber>(TNumber value, TNumber exponent)
      => (TNumber) (dynamic) CMath.Pow((double) (dynamic) value!,
                                       (double) (dynamic) exponent!);

    /**
     * Working w/ averages
     */
    public static TNumber Mean<TNumber>(params TNumber[] values)
        where TNumber : IComparable {
      dynamic runningMean = 0;
      var multiplier = 1f / values.Length;

      foreach (var value in values) {
        runningMean += (dynamic)value * multiplier;
      }

      return (TNumber)runningMean;
    }

    /**
     * Working w/ signs
     */
    public static TNumber Abs<TNumber>(TNumber value)
        where TNumber : IComparable
      => Math.IsPositive(value)
             ? value
             : -(dynamic) value;

    public static int Sign<TNumber>(TNumber value)
        where TNumber : IComparable
      => Math.IsPositive(value) ? 1 : Math.IsNegative(value) ? -1 : 0;

    public static bool IsPositive<TNumber>(TNumber value)
        where TNumber : IComparable
      => value.CompareTo((dynamic) value * 0) > 0;

    public static bool IsNegative<TNumber>(TNumber value)
        where TNumber : IComparable
      => value.CompareTo((dynamic) value * 0) < 0;

    /**
     * Checking number ranges
     */
    public static bool IsGreaterThan<TNumber>(TNumber lhs, TNumber rhs)
        where TNumber : IComparable
      => lhs.CompareTo(rhs) > 0;

    public static bool IsGreaterThanOrEqualTo<TNumber>(TNumber lhs, TNumber rhs)
        where TNumber : IComparable
      => lhs.CompareTo(rhs) >= 0;

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

    public static bool IsIncreasing<TNumber>(params TNumber[] values)
        where TNumber : IComparable {
      var previousValue = values[0];
      foreach (var value in values) {
        if (Math.IsGreaterThan(previousValue, value)) {
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
        if (Math.IsLessThan(previousValue, value)) {
          return false;
        }
        previousValue = value;
      }
      return true;
    }

    /**
     * Applying value ranges
     */
    public static TNumber Min<TNumber>(TNumber lhs, TNumber rhs)
        where TNumber : IComparable
      => Math.IsLessThan(lhs, rhs) ? lhs : rhs;

    public static TNumber Max<TNumber>(TNumber lhs, TNumber rhs)
        where TNumber : IComparable
      => Math.IsGreaterThan(lhs, rhs) ? lhs : rhs;

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

    public static TNumber AddTowards<TNumber>(
        TNumber start,
        TNumber end,
        TNumber inc) where TNumber : IComparable {
      Asserts.True(Math.IsPositive(inc), "Expected increment to be positive.");

      dynamic dStart = start;
      dynamic dEnd = end;
      dynamic dInc = inc;

      var range = dEnd - dStart;

      if (Math.Abs(range) < dInc) {
        return end;
      }

      var sign = Math.Sign(range);
      return dStart + sign * inc;
    }
  }
}