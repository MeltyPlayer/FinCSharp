using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.data.collections.set {
  public abstract class BFinSetTest : BFinCollectionTest {
    protected abstract IFinSet<T> NewEmptySet<T>();

    protected override IFinCollection<T> NewEmptyCollection<T>() =>
        this.NewEmptySet<T>();

    [TestMethod]
    public void TestAdd() {
      var emptySet = this.NewEmptySet<int>();
      Assert.IsFalse(emptySet.Contains(3));

      Assert.IsTrue(emptySet.Add(3));
      Assert.IsTrue(emptySet.Contains(3));

      Assert.IsFalse(emptySet.Add(3));
    }

    [TestMethod]
    public void TestRemove() {
      var emptySet = this.NewEmptySet<int>();
      Assert.IsFalse(emptySet.Remove(3));

      emptySet.Add(3);
      
      Assert.IsTrue(emptySet.Remove(3));
      Assert.IsFalse(emptySet.Contains(3));

      Assert.IsFalse(emptySet.Remove(3));
    }
  }
}