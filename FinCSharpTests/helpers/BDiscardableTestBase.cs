using fin.assert.fluent;
using fin.discardable;
using fin.discardable.impl;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers {
  [TestClass]
  public abstract class BDiscardableTestBase {
    private DiscardableNodeFactoryImpl? factory_;
    private IDiscardableNode rootDiscardable_;

    [TestInitialize]
    public void BeforeEach() {
      this.factory_ =
          new DiscardableNodeFactoryImpl(
              rootDiscardable =>
                  this.OnRootDiscardableCreated(
                      this.rootDiscardable_ = rootDiscardable));
    }

    [TestCleanup]
    public void AfterEach() {
      this.rootDiscardable_.Discard();
      Expect.That(this.factory_!.Count).Equals(0);
    }

    protected abstract void OnRootDiscardableCreated(
        IDiscardableNode rootDiscardable);
  }
}