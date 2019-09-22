using System;
using System.Diagnostics;

namespace fin.assert {
  public static class Assert {
    public static void True(bool value,
      string message = "Expected to be true.") {
      Trace.Assert(value, message);
    }

    public static void False(bool value,
      string message = "Expected to be false.") {
      Assert.True(!value, message);
    }

    public static void Nonnull(object? instance,
      string message = "Expected reference to be nonnull.") {
      Assert.True(instance != null, message);
    }

    public static void Null(object? instance,
      string message = "Expected reference to be null.") {
      Assert.True(instance == null, message);
    }

    public static void Same(object instanceA,
      object instanceB,
      string message = "Expected references to be the same.") {
      Assert.True(object.ReferenceEquals(instanceA, instanceB), message);
    }

    public static void Different(object instanceA,
      object instanceB,
      string message = "Expected references to be different.") {
      Assert.False(object.ReferenceEquals(instanceA, instanceB), message);
    }

    public static void Equal(object instanceA,
      object instanceB,
      string message = "Expected to be equal.") {
      Assert.True(instanceA == instanceB, message);
    }
  }
}