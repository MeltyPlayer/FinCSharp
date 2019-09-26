using System.Collections;
using System.Diagnostics;

namespace fin.assert {

  public static class Asserts {

    public static void True(bool value, string message = "Expected to be true.") {
      if (!value) {
        throw new AssertionException(message);
      }
    }

    public static void False(bool value, string message = "Expected to be false.") {
      Asserts.True(!value, message);
    }

    public static void Nonnull(object? instance, string message = "Expected reference to be nonnull.") {
      Asserts.True(instance != null, message);
    }

    public static void Null(object? instance, string message = "Expected reference to be null.") {
      Asserts.True(instance == null, message);
    }

    public static void Same(object instanceA, object instanceB, string message = "Expected references to be the same.") {
      Asserts.True(object.ReferenceEquals(instanceA, instanceB), message);
    }

    public static void Different(object instanceA, object instanceB, string message = "Expected references to be different.") {
      Asserts.False(object.ReferenceEquals(instanceA, instanceB), message);
    }

    public static void Equal(object? instanceA, object? instanceB, string message = "") {
      Asserts.True(instanceA == instanceB, message ?? $"Expected {instanceA} to equal {instanceB}.");
    }

    public static void Equal(IEnumerable enumerableA, IEnumerable enumerableB) {
      var enumeratorA = enumerableA.GetEnumerator();
      var enumeratorB = enumerableB.GetEnumerator();

      var hasA = enumeratorA.MoveNext();
      var hasB = enumeratorB.MoveNext();

      var index = 0;
      while (hasA && hasB) {
        var currentA = enumeratorA.Current;
        var currentB = enumeratorB.Current;
        Asserts.True(object.Equals(currentA, currentB), $"Expected {currentA} to equal {currentB} at index ${index}.");
        index++;

        hasA = enumeratorA.MoveNext();
        hasB = enumeratorB.MoveNext();
      }

      Asserts.True(!hasA && !hasB, "Expected enumerables to be the same length.");
    }
  }
}