using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.pointer.contract {
  [TestClass]
  public class StrongContractSetTest {
    private static readonly IContractFactory
      FACTORY = IContractFactory.Instance;

    [TestMethod]
    public void TestSetInitiallyEmpty() {
      var set = FACTORY.NewStrongOwner<int>();

      Utils.AssertEmpty(set);
    }

    [TestMethod]
    public void TestSingleJoinFromContract() {
      var set = FACTORY.NewStrongOwner<int>();

      var contract = set.FormOpen(0);

      Utils.AssertContents(set, contract);
    }

    [TestMethod]
    public void TestSingleBreakFromContract() {
      var set = FACTORY.NewStrongOwner<int>();
      var contract = set.FormOpen(0);
      Assert.IsTrue(contract.IsActive);

      Assert.IsTrue(contract.Break());
      Assert.IsFalse(contract.IsActive);
      Utils.AssertEmpty(set);

      Assert.IsFalse(contract.Break());
    }

    [TestMethod]
    public void TestSingleBreakFromSet() {
      var set = FACTORY.NewStrongOwner<int>();
      var contract = set.FormOpen(0);
      Assert.IsTrue(contract.IsActive);

      Assert.IsTrue(set.Break(contract));
      Assert.IsFalse(set.Break(contract));

      Assert.IsFalse(contract.IsActive);
      Utils.AssertEmpty(set);
    }

    [TestMethod]
    public void TestMultipleJoinFromContract() {
      var setA = FACTORY.NewStrongOwner<int>();
      var setB = FACTORY.NewStrongOwner<int>();

      var contract = setA.FormOpen(0);
      Assert.IsFalse(contract.Join(setA));
      Assert.IsTrue(contract.Join(setB));
      Assert.IsFalse(contract.Join(setB));

      Utils.AssertContents(setA, contract);
      Utils.AssertContents(setB, contract);
    }

    [TestMethod]
    public void TestMultipleJoinFromSet() {
      var setA = FACTORY.NewStrongOwner<int>();
      var setB = FACTORY.NewStrongOwner<int>();

      var contract = setA.FormOpen(0);
      Assert.IsFalse(setA.Join(contract));
      Assert.IsTrue(setB.Join(contract));
      Assert.IsFalse(setB.Join(contract));

      Utils.AssertContents(setA, contract);
      Utils.AssertContents(setB, contract);
    }

    [TestMethod]
    public void TestMultipleBreakFromContract() {
      var setA = FACTORY.NewStrongOwner<int>();
      var setB = FACTORY.NewStrongOwner<int>();
      var contract = setA.FormClosedWith(0, setB);
      Assert.IsTrue(contract.IsActive);

      Assert.IsTrue(contract.Break());
      Assert.IsFalse(contract.Break());

      Assert.IsFalse(contract.IsActive);
      Utils.AssertEmpty(setA);
      Utils.AssertEmpty(setB);
    }

    [TestMethod]
    public void TestMultipleBreakFromSet() {
      var setA = FACTORY.NewStrongOwner<int>();
      var setB = FACTORY.NewStrongOwner<int>();
      var contract = setA.FormClosedWith(0, setB);
      Assert.IsTrue(contract.IsActive);

      Assert.IsTrue(setA.Break(contract));
      Assert.IsTrue(contract.IsActive);
      Assert.IsFalse(setA.Break(contract));

      Assert.IsTrue(setB.Break(contract));
      Assert.IsFalse(setB.Break(contract));

      Assert.IsFalse(contract.IsActive);
      Utils.AssertEmpty(setA);
      Utils.AssertEmpty(setB);
    }
  }
}