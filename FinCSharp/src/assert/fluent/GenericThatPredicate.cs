using System;

namespace fin.assert.fluent {
  public sealed partial class Expect {
    public static GenericThatPredicate<T> That<T>(T lhs)
      => new GenericThatPredicate<T>(lhs);

    public class GenericThatPredicate<T> : IThatPredicate<T> {
      private readonly T lhs_;

      public GenericThatPredicate(T lhs) {
        this.lhs_ = lhs;
      }

      public bool Equals(T rhs, string? message = null)
        => Asserts.Equal(this.lhs_, rhs, message);

      public bool IsA<TRhs>(string? message = null)
        => Asserts.IsA<TRhs>(this.lhs_, message);

      public bool IsA(Type rhs, string? message = null)
        => Asserts.IsA(this.lhs_, rhs, message);

      public TRhs AsA<TRhs>(string? message = null)
        => Asserts.AsA<TRhs>(this.lhs_, message);
    }
  }
}