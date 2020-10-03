using System;

using fin.assert.fluent;
using fin.helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.discardable {
  [TestClass]
  public class DiscardableNodeImplTest : BDiscardableTestBase {
    private IDiscardableNode rootDiscardable_ = default;

    protected override void OnRootDiscardableCreated(
        IDiscardableNode rootDiscardable)
      => this.rootDiscardable_ = rootDiscardable;

    [TestMethod]
    public void TestDiscard() {
      var d = this.rootDiscardable_.CreateChild();

      Assert.IsFalse(d.IsDiscarded);

      Assert.IsTrue(d.Discard());
      Assert.IsTrue(d.IsDiscarded);

      Assert.IsFalse(d.Discard());
      Assert.IsTrue(d.IsDiscarded);
    }

    [TestMethod]
    public void TestOnDiscard() {
      var d = this.rootDiscardable_.CreateChild();

      var output = "";
      d.OnDiscard += _ => output += "foobar";

      // Text should be added on first discard.
      d.Discard();
      Assert.AreEqual("foobar", output);

      // Nothing should happen on successive discards.
      output = "";
      d.Discard();
      Assert.AreEqual("", output);
    }

    [TestMethod]
    public void TestDependentCreateChild() {
      var parent = this.rootDiscardable_.CreateChild();
      var d = parent.CreateChild();

      var output = "";
      d.OnDiscard += _ => output += "foobar";

      // Text should be added on first discard.
      parent.Discard();
      Assert.IsTrue(parent.IsDiscarded);
      Assert.IsTrue(d.IsDiscarded);
      Assert.AreEqual("foobar", output);

      // Nothing should happen on successive discards.
      output = "";
      parent.Discard();
      Assert.AreEqual("", output);
    }

    [TestMethod]
    public void TestDependentSetParent() {
      var parent = this.rootDiscardable_.CreateChild();
      var d = this.rootDiscardable_.CreateChild();

      d.SetParent(parent);

      var output = "";
      d.OnDiscard += _ => output += "foobar";

      // Text should be added on first discard.
      parent.Discard();
      Assert.IsTrue(parent.IsDiscarded);
      Assert.IsTrue(d.IsDiscarded);
      Assert.AreEqual("foobar", output);

      // Nothing should happen on successive discards.
      output = "";
      parent.Discard();
      Assert.AreEqual("", output);
    }

    class TestableDisposable : IDisposable {
      private bool isDisposed_;
      private readonly Action handler_;

      public TestableDisposable(Action handler) {
        this.handler_ = handler;
      }

      protected virtual void Dispose_(bool disposing) {
        if (!this.isDisposed_) {
          if (disposing) {
            this.handler_();
          }

          this.isDisposed_ = true;
        }
      }

      public void Dispose() {
        this.Dispose_(disposing: true);
        GC.SuppressFinalize(this);
      }
    }

    [TestMethod]
    public void TestUsing() {
      var d = this.rootDiscardable_.CreateChild();

      var output = "";
      d.Using(new TestableDisposable(() => output += "foobar"));
      Expect.That(output).Equals("");

      d.Discard();
      Expect.That(output).Equals("foobar");
    }
  }
}