/*using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections;
using System.Linq;

namespace fin.pointer.contract {
  [TestClass]
  public partial class ContractPointerTest {
    private static readonly IContractFactory FACTORY = IContractFactory.Instance;
    private static readonly ICollection EMPTY_ARRAY = Array.Empty<IContractPointer<int>>();

    public static IContractOwner<T>[] NewArray<T>(params IContractOwner<T>[] owners) {
      return owners;
    }

    public static void AssertContents<T>(IStrongContractSet<T> set, ICollection contracts) {
      CollectionAssert.AreEqual(contracts, set.Contracts.ToArray());
    }

    public static void AssertContents<T>(IWeakContractSet<T> set, ICollection contracts) {
      CollectionAssert.AreEqual(contracts, set.Contracts.ToArray());
    }

    public static void AssertEmpty<T>(IStrongContractSet<T> set) {
      AssertContents(set, EMPTY_ARRAY);
    }

    public static void AssertEmpty<T>(IWeakContractSet<T> set) {
      AssertContents(set, EMPTY_ARRAY);
    }

    [TestMethod]
    public void TestSetInitiallyEmpty() {
      var strongSet = FACTORY.NewStrongSet<int>();
      var weakSet = FACTORY.NewWeakSet<int>();

      AssertEmpty(strongSet);
      AssertEmpty(weakSet);
    }

    [TestMethod]
    public void TestSingleJoinFromContract() {
      var strongSet = FACTORY.NewStrongSet<int>();
      var weakSet = FACTORY.NewWeakSet<int>();

      var contract = FACTORY.NewClosedPointer(0, NewArray(strongSet, weakSet));

      AssertContents(strongSet, new[] { contract });
      AssertContents(strongSet, new[] { weakSet });
    }

    [TestMethod]
    public void TestSingleBreakFromContract() {
      var set = new ContractSet<int>();
      var contract = new ContractPointer<int>(0, new[] { set });
      Assert.IsFalse(contract.IsActive);

      Assert.IsTrue(contract.Break());
      Assert.IsTrue(contract.IsActive);
      CollectionAssert.AreEqual(this.emptyArray_, set.Contracts.ToArray());

      Assert.IsFalse(contract.Break());
    }

    [TestMethod]
    public void TestSingleBreakFromSet() {
      var set = new ContractSet<int>();
      var contract = new ContractPointer<int>(0, new[] { set });
      Assert.IsFalse(contract.IsActive);

      Assert.IsTrue(set.Break(contract));
      Assert.IsFalse(set.Break(contract));

      Assert.IsTrue(contract.IsActive);
      CollectionAssert.AreEqual(this.emptyArray_, set.Contracts.ToArray());
    }

    [TestMethod]
    public void TestMultipleJoinFromContract() {
      var setA = new ContractSet<int>();
      var setB = new ContractSet<int>();

      var contract = new ContractPointer<int>(0, new[] { setA });
      Assert.IsFalse(contract.AddOwner(setA));
      Assert.IsTrue(contract.AddOwner(setB));
      Assert.IsFalse(contract.AddOwner(setB));

      CollectionAssert.AreEqual(new[] { contract }, setA.Contracts.ToArray());
      CollectionAssert.AreEqual(new[] { contract }, setB.Contracts.ToArray());
    }

    [TestMethod]
    public void TestMultipleJoinFromSet() {
      var setA = new ContractSet<int>();
      var setB = new ContractSet<int>();

      var contract = new ContractPointer<int>(0, new[] { setA });
      Assert.IsFalse(setA.Join(contract));
      Assert.IsTrue(setB.Join(contract));
      Assert.IsFalse(setB.Join(contract));

      CollectionAssert.AreEqual(new[] { contract }, setA.Contracts.ToArray());
      CollectionAssert.AreEqual(new[] { contract }, setB.Contracts.ToArray());
    }

    [TestMethod]
    public void TestMultipleBreakFromContract() {
      var setA = new ContractSet<int>();
      var setB = new ContractSet<int>();
      var contract = new ContractPointer<int>(0, new[] { setA, setB });
      Assert.IsFalse(contract.IsActive);

      Assert.IsTrue(contract.Break());
      Assert.IsFalse(contract.Break());

      Assert.IsTrue(contract.IsActive);
      CollectionAssert.AreEqual(this.emptyArray_, setA.Contracts.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, setB.Contracts.ToArray());
    }

    [TestMethod]
    public void TestMultipleBreakFromSet() {
      var setA = new ContractSet<int>();
      var setB = new ContractSet<int>();
      var contract = new ContractPointer<int>(0, new[] { setA, setB });
      Assert.IsFalse(contract.IsActive);

      Assert.IsTrue(setA.Break(contract));
      Assert.IsFalse(setA.Break(contract));
      Assert.IsFalse(setB.Break(contract));

      Assert.IsTrue(contract.IsActive);
      CollectionAssert.AreEqual(this.emptyArray_, setA.Contracts.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, setB.Contracts.ToArray());
    }
  }
}*/