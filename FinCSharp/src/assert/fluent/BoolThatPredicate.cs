namespace fin.assert.fluent {
  public sealed partial class Expect {
    public static BoolThatPredicate That(bool lhs) =>
        new BoolThatPredicate(lhs);

    public class BoolThatPredicate : IThatPredicate<bool> {
      private readonly bool lhs_;

      public BoolThatPredicate(bool lhs) {
        this.lhs_ = lhs;
      }

      public bool Equals(bool rhs, string message = "")
        => Asserts.Equal(this.lhs_, rhs, message);

      public void IsTrue(string message = "") => this.Equals(true, message);
      public void IsFalse(string message = "") => this.Equals(false, message);
    }
  }
}