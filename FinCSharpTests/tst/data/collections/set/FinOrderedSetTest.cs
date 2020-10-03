using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.data.collections.set {
  [TestClass]
  public class FinOrderedSetTest : BFinSetTestBase {
    protected override IFinSet<T> NewEmptySet<T>() => new FinOrderedSet<T>();
  }
}