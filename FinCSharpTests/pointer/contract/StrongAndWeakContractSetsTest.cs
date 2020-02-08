using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.pointer.contract {

  [TestClass]
  public class StrongAndWeakContractSetsTest {
    private static readonly IContractFactory FACTORY = IContractFactory.Instance;

    [TestMethod]
    public void TestJoinFromContract() {
      var setA = FACTORY.NewStrongSet<int>();
      var setB = FACTORY.NewWeakSet<int>();

      var contract = setA.FormOpen(0);
      Assert.IsFalse(contract.Join(setA));
      Assert.IsTrue(contract.Join(setB));
      Assert.IsFalse(contract.Join(setB));

      Utils.AssertContents(setA, contract);
      Utils.AssertContents(setB, contract);
    }

    [TestMethod]
    public void TestJoinFromSet() {
      var setA = FACTORY.NewStrongSet<int>();
      var setB = FACTORY.NewWeakSet<int>();

      var contract = setA.FormOpen(0);
      Assert.IsFalse(setA.Join(contract));
      Assert.IsTrue(setB.Join(contract));
      Assert.IsFalse(setB.Join(contract));

      Utils.AssertContents(setA, contract);
      Utils.AssertContents(setB, contract);
    }

    [TestMethod]
    public void TestBreakFromContract() {
      var setA = FACTORY.NewStrongSet<int>();
      var setB = FACTORY.NewWeakSet<int>();
      var contract = setA.FormClosedWith(0, setB);
      Assert.IsTrue(contract.IsActive);

      Assert.IsTrue(contract.Break());
      Assert.IsFalse(contract.Break());

      Assert.IsFalse(contract.IsActive);
      Utils.AssertEmpty(setA);
      Utils.AssertEmpty(setB);
    }

    [TestMethod]
    public void TestBreakFromSetStrongFirst() {
      var setA = FACTORY.NewStrongSet<int>();
      var setB = FACTORY.NewWeakSet<int>();
      var contract = setA.FormClosedWith(0, setB);
      Assert.IsTrue(contract.IsActive);

      Assert.IsTrue(setA.Break(contract));
      Assert.IsFalse(contract.IsActive);

      Assert.IsFalse(setA.Break(contract));
      Assert.IsFalse(setB.Break(contract));

      Utils.AssertEmpty(setA);
      Utils.AssertEmpty(setB);
    }

    [TestMethod]
    public void TestBreakFromSetWeakFirst() {
      var setA = FACTORY.NewStrongSet<int>();
      var setB = FACTORY.NewWeakSet<int>();
      var contract = setA.FormClosedWith(0, setB);
      Assert.IsTrue(contract.IsActive);

      Assert.IsTrue(setB.Break(contract));
      Assert.IsTrue(contract.IsActive);
      Assert.IsFalse(setB.Break(contract));

      Assert.IsTrue(setA.Break(contract));
      Assert.IsFalse(setA.Break(contract));

      Assert.IsFalse(contract.IsActive);
      Utils.AssertEmpty(setA);
      Utils.AssertEmpty(setB);
    }
  }
}