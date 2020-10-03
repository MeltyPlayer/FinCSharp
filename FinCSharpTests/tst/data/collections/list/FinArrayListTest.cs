using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.data.collections.list {
  [TestClass]
  public class FinArrayListTest : BFinListTestBase {
    protected override IFinList<T> NewEmptyList<T>() => new FinArrayList<T>();

    [TestMethod]
    public void TestSizeConstructor() {
      var list = new FinArrayList<int>(1);
      Assert.AreEqual(1, list.Count);
      Assert.AreEqual(0, list[0]);

      list[0] = 3;
      Assert.AreEqual(3, list[0]);
    }
  }
}