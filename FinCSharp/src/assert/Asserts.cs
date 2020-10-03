using System;
using System.Collections;

namespace fin.assert {
  public static class Asserts {
    public static bool Fail(string? message = null)
      => throw new AssertionException(message ?? "Failed.");

    public static bool True(bool value, string? message = null)
      => value || Asserts.Fail(message ?? "Expected to be true.");

    public static bool False(bool value, string? message = null)
      => Asserts.True(!value, message ?? "Expected to be false.");

    public static bool Nonnull(
        object? instance,
        string? message = null)
      => Asserts.True(instance != null,
                      message ?? "Expected reference to be nonnull.");

    public static void Null(
        object? instance,
        string message = "Expected reference to be null.")
      => Asserts.True(instance == null, message);

    public static bool Same(
        object instanceA,
        object instanceB,
        string message = "Expected references to be the same.")
      => Asserts.True(object.ReferenceEquals(instanceA, instanceB), message);

    public static void Different(
        object instanceA,
        object instanceB,
        string message = "Expected references to be different.") {
      Asserts.False(object.ReferenceEquals(instanceA, instanceB), message);
    }

    public static bool Equal(
        object? instanceA,
        object? instanceB,
        string? message = null)
      => Asserts.True(instanceA?.Equals(instanceB) ?? false,
                      message ?? $"Expected {instanceA} to equal {instanceB}.");

    public static void Equal<TEnumerable>(
        TEnumerable enumerableA,
        TEnumerable enumerableB) where TEnumerable : IEnumerable {
      var enumeratorA = enumerableA.GetEnumerator();
      var enumeratorB = enumerableB.GetEnumerator();

      var hasA = enumeratorA.MoveNext();
      var hasB = enumeratorB.MoveNext();

      var index = 0;
      while (hasA && hasB) {
        var currentA = enumeratorA.Current;
        var currentB = enumeratorB.Current;
        Asserts.True(object.Equals(currentA, currentB),
                     $"Expected {currentA} to equal {currentB} at index ${index}.");
        index++;

        hasA = enumeratorA.MoveNext();
        hasB = enumeratorB.MoveNext();
      }

      Asserts.True(!hasA && !hasB,
                   "Expected enumerables to be the same length.");
    }

    public static bool Equal<T>(
        T instanceA,
        T instanceB,
        string? message = null)
      => Asserts.True(instanceA?.Equals(instanceB) ?? false,
                      message ?? $"Expected {instanceA} to equal {instanceB}.");

    public static bool IsA<TExpected>(object? instance, string? message = null)
      => Asserts.IsA(instance, typeof(TExpected), message);

    public static bool IsA(
        object? instance,
        Type expected,
        string? message = null)
      => Asserts.Nonnull(instance, message) &&
         Asserts.Equal(instance!.GetType(), expected, message);

    public static TExpected AsA<TExpected>(
        object? instance,
        string? message = null) {
      Asserts.IsA<TExpected>(instance, message);
      return (TExpected) instance!;
    }
  }
}