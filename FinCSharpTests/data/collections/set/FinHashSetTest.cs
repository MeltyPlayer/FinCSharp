using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.data.collections.set {
  [TestClass]
  public class FinHashSetTest : BFinSetTest {
    protected override IFinSet<T> NewEmptySet<T>() => new FinHashSet<T>();
  }
}
