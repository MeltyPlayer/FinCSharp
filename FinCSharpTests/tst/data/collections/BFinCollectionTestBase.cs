using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.data.collections {
  public abstract class BFinCollectionTestBase {
    protected abstract IFinCollection<T> NewEmptyCollection<T>();

    [TestMethod]
    public void TestEmptySizeInitiallyZero() {
      var emptyCollection = this.NewEmptyCollection<int>();
      Assert.AreEqual(emptyCollection.Count, 0);
    }
  }
}
