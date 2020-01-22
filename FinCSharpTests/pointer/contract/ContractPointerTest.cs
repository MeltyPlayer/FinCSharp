using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections;
using System.Linq;

namespace fin.pointer.contract {

  [TestClass]
  public partial class ContractPointerTest {
    private readonly ICollection emptyArray_ = Array.Empty<IContractPointer<int>>();

    [TestMethod]
    public void TestSetInitiallyEmpty() {
      var set = new ContractSet<int>();

      CollectionAssert.AreEqual(this.emptyArray_, set.Contracts.ToArray());
    }

    [TestMethod]
    public void TestSingleJoinFromContract() {
      var set = new ContractSet<int>();

      var contract = new ContractPointer<int>(0, new[] { set });

      CollectionAssert.AreEqual(new[] { contract }, set.Contracts.ToArray());
    }

    [TestMethod]
    public void TestSingleBreakFromContract() {
      var set = new ContractSet<int>();
      var contract = new ContractPointer<int>(0, new[] { set });
      Assert.IsFalse(contract.IsBroken);

      Assert.IsTrue(contract.Break());
      Assert.IsTrue(contract.IsBroken);
      CollectionAssert.AreEqual(this.emptyArray_, set.Contracts.ToArray());

      Assert.IsFalse(contract.Break());
    }

    [TestMethod]
    public void TestSingleBreakFromSet() {
      var set = new ContractSet<int>();
      var contract = new ContractPointer<int>(0, new[] { set });
      Assert.IsFalse(contract.IsBroken);

      Assert.IsTrue(set.Break(contract));
      Assert.IsFalse(set.Break(contract));

      Assert.IsTrue(contract.IsBroken);
      CollectionAssert.AreEqual(this.emptyArray_, set.Contracts.ToArray());
    }

    [TestMethod]
    public void TestMultipleJoinFromContract() {
      var setA = new ContractSet<int>();
      var setB = new ContractSet<int>();

      var contract = new ContractPointer<int>(0, new[] { setA });
      Assert.IsFalse(contract.Join(setA));
      Assert.IsTrue(contract.Join(setB));
      Assert.IsFalse(contract.Join(setB));

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
      Assert.IsFalse(contract.IsBroken);

      Assert.IsTrue(contract.Break());
      Assert.IsFalse(contract.Break());

      Assert.IsTrue(contract.IsBroken);
      CollectionAssert.AreEqual(this.emptyArray_, setA.Contracts.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, setB.Contracts.ToArray());
    }

    [TestMethod]
    public void TestMultipleBreakFromSet() {
      var setA = new ContractSet<int>();
      var setB = new ContractSet<int>();
      var contract = new ContractPointer<int>(0, new[] { setA, setB });
      Assert.IsFalse(contract.IsBroken);

      Assert.IsTrue(setA.Break(contract));
      Assert.IsFalse(setA.Break(contract));
      Assert.IsFalse(setB.Break(contract));

      Assert.IsTrue(contract.IsBroken);
      CollectionAssert.AreEqual(this.emptyArray_, setA.Contracts.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, setB.Contracts.ToArray());
    }
  }
}