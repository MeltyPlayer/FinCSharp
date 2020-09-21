namespace fin.assert.fluent {
  public sealed partial class Expect {
    public static IThatPredicate<T> That<T>(T lhs) => new ThatPredicate<T>(lhs);

    public interface IThatPredicate<T> {
      void Equals(T rhs, string message = "");
    }

    public class ThatPredicate<T> : IThatPredicate<T> {
      private readonly T lhs_;

      public ThatPredicate(T lhs) {
        this.lhs_ = lhs;
      }

      public void Equals(T rhs, string message = "") 
        => Asserts.Equal(this.lhs_, rhs, message);
    }
  }
}