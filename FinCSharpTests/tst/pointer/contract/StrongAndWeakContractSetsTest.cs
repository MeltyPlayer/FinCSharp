using fin.discardable;
using fin.helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.pointer.contract {
  [TestClass]
  public class StrongAndWeakContractSetsTest : BContractTestBase {
    private IDiscardableNode rootDiscardable_ = default;

    private static readonly IContractFactory
        FACTORY = IContractFactory.INSTANCE;

    protected override void OnRootDiscardableCreated(
        IDiscardableNode rootDiscardable)
      => this.rootDiscardable_ = rootDiscardable;

    [TestMethod]
    public void TestJoinFromContract() {
      var setA = FACTORY.NewStrongOwner<int>(this.rootDiscardable_);
      var setB = FACTORY.NewWeakOwner<int>(this.rootDiscardable_);

      var contract = setA.FormOpen(0);
      Assert.IsFalse(contract.Join(setA));
      Assert.IsTrue(contract.Join(setB));
      Assert.IsFalse(contract.Join(setB));

      Utils.AssertContents(setA, contract);
      Utils.AssertContents(setB, contract);
    }

    [TestMethod]
    public void TestJoinFromSet() {
      var setA = FACTORY.NewStrongOwner<int>(this.rootDiscardable_);
      var setB = FACTORY.NewWeakOwner<int>(this.rootDiscardable_);

      var contract = setA.FormOpen(0);
      Assert.IsFalse(setA.Join(contract));
      Assert.IsTrue(setB.Join(contract));
      Assert.IsFalse(setB.Join(contract));

      Utils.AssertContents(setA, contract);
      Utils.AssertContents(setB, contract);
    }

    [TestMethod]
    public void TestBreakFromContract() {
      var setA = FACTORY.NewStrongOwner<int>(this.rootDiscardable_);
      var setB = FACTORY.NewWeakOwner<int>(this.rootDiscardable_);
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
      var setA = FACTORY.NewStrongOwner<int>(this.rootDiscardable_);
      var setB = FACTORY.NewWeakOwner<int>(this.rootDiscardable_);
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
      var setA = FACTORY.NewStrongOwner<int>(this.rootDiscardable_);
      var setB = FACTORY.NewWeakOwner<int>(this.rootDiscardable_);
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