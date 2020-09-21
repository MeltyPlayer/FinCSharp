using System;

namespace fin.assert.fluent {
  public sealed partial class Expect {
    public static ActionThatPredicate That(Action lhs) =>
        new ActionThatPredicate(lhs);

    public class ActionThatPredicate : IThatPredicate<Action> {
      private readonly Action lhs_;

      public ActionThatPredicate(Action lhs) {
        this.lhs_ = lhs;
      }

      public void Equals(Action rhs, string message = "")
        => Asserts.Equal(this.lhs_, rhs, message);

      public void Throws<TException>(TException expected)
          where TException : Exception {
        try {
          this.Execute_();
        }
        catch (TException actual) {
          Asserts.Equal(expected, actual);
          return;
        }

        Asserts.Fail();
      }

      private void Execute_() => this.lhs_();
    }
  }
}