namespace fin.assert.fluent {
  public sealed partial class Expect {
    public static BoolThatPredicate That(bool lhs) =>
        new BoolThatPredicate(lhs);

    public class BoolThatPredicate : ThatPredicate<bool> {
      public BoolThatPredicate(bool lhs) : base(lhs) { }

      public void IsTrue(string message = "") => this.Equals(true, message);
      public void IsFalse(string message = "") => this.Equals(false, message);
    }
  }
}