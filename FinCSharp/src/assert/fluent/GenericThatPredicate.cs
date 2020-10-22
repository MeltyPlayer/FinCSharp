using System;

namespace fin.assert.fluent {
  public sealed partial class Expect {
    public static GenericThatPredicate<T> That<T>(T actual)
      => new GenericThatPredicate<T>(actual);

    public class GenericThatPredicate<T> : IThatPredicate<T> {
      private readonly T actual_;

      public GenericThatPredicate(T actual) {
        this.actual_ = actual;
      }

      public bool Equals(T expected, string? message = null)
        => Asserts.Equal(expected, this.actual_, message);

      public bool IsA<TRhs>(string? message = null)
        => Asserts.IsA<TRhs>(this.actual_, message);

      public bool IsA(Type expected, string? message = null)
        => Asserts.IsA(this.actual_, expected, message);

      public TRhs AsA<TRhs>(string? message = null)
        => Asserts.AsA<TRhs>(this.actual_, message);
    }
  }
}