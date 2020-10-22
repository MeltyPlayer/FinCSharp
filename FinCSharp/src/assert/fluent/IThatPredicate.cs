namespace fin.assert.fluent {
  public sealed partial class Expect {
    public interface IThatPredicate<T> {
      bool Equals(T expected, string message = "");
    }
  }
}