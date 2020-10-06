using fin.assert.fluent;
using fin.discardable;
using fin.discardable.impl;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers {
  public abstract class BDiscardableTestBase {
    private DiscardableNodeFactoryImpl? discardableFactory_;
    private IDiscardableNode rootDiscardable_;

    [TestInitialize]
    public void BeforeEachDiscardableTest() {
      this.discardableFactory_ =
          new DiscardableNodeFactoryImpl(
              rootDiscardable =>
                  this.OnRootDiscardableCreated(
                      this.rootDiscardable_ = rootDiscardable));
    }

    protected abstract void OnRootDiscardableCreated(
        IDiscardableNode rootDiscardable);

    [TestCleanup]
    public void AfterEachDiscardableTest() {
      this.rootDiscardable_.Discard();
      Expect.That(this.discardableFactory_!.Count).Equals(0);

      this.OnAfterEachDiscardableTest();
    }

    public virtual void OnAfterEachDiscardableTest() {}
  }
}