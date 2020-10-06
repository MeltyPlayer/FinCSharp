using fin.assert.fluent;
using fin.pointer.contract;

namespace fin.helpers {
  public abstract class BContractTestBase : BDiscardableTestBase {
    public override void OnAfterEachDiscardableTest() {
      Expect.That(IContractFactory.INSTANCE.Count).Equals(0);
    }
  }
}