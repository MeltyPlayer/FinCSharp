namespace fin.assert.fluent {
  public sealed partial class Expect {
    public static BoolThatPredicate That(bool actual) =>
        new BoolThatPredicate(actual);

    public class BoolThatPredicate : IThatPredicate<bool> {
      private readonly bool actual_;

      public BoolThatPredicate(bool actual) {
        this.actual_ = actual;
      }

      public bool Equals(bool expected, string message = "")
        => Asserts.Equal(expected, this.actual_, message);

      public void IsTrue(string message = "") => this.Equals(true, message);
      public void IsFalse(string message = "") => this.Equals(false, message);
    }
  }
}