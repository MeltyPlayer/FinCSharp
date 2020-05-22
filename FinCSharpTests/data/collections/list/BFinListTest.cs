using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.data.collections.list {
  public abstract class BFinListTest : BFinCollectionTest {
    protected abstract IFinList<T> NewEmptyList<T>();

    protected override IFinCollection<T> NewEmptyCollection<T>() =>
        this.NewEmptyList<T>();

    [TestMethod]
    public void TestCannotGetPastBounds() {
      var emptyList = this.NewEmptyList<int>();
      Assert.ThrowsException<ArgumentOutOfRangeException>(() => emptyList[0]);
    }

    [TestMethod]
    public void TestCannotSetPastBounds() {
      var emptyList = this.NewEmptyList<int>();
      Assert.ThrowsException<ArgumentOutOfRangeException>(
          () => emptyList[0] = 1);
    }
  }
}