using System;

namespace fin.assert.fluent {
  public sealed partial class Expect {
    public static ActionThatPredicate That(Action actual) =>
        new ActionThatPredicate(actual);

    public class ActionThatPredicate : IThatPredicate<Action> {
      private readonly Action actual_;

      public ActionThatPredicate(Action actual) {
        this.actual_ = actual;
      }

      public bool Equals(Action expected, string message = "")
        => Asserts.Equal(expected, this.actual_, message);

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

      private void Execute_() => this.actual_();
    }
  }
}