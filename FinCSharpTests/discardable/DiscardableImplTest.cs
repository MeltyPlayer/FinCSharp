using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.discardable {
  [TestClass]
  public class DiscardableImplTest {
    [TestMethod]
    public void TestDiscard() {
      var d = new DiscardableImpl();

      Assert.IsFalse(d.IsDiscarded);

      Assert.IsTrue(d.Discard());
      Assert.IsTrue(d.IsDiscarded);

      Assert.IsFalse(d.Discard());
      Assert.IsTrue(d.IsDiscarded);
    }

    [TestMethod]
    public void TestOnDiscard() {
      var d = new DiscardableImpl();

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
    public void TestDependentAddAndRemove() {
      var parent = new DiscardableImpl();
      var d = new DiscardableImpl();

      // Can only add/remove once.
      Assert.IsTrue(d.AddParent(parent));
      Assert.IsFalse(d.AddParent(parent));

      Assert.IsTrue(d.RemoveParent(parent));
      Assert.IsFalse(d.RemoveParent(parent));

      // (Discard removes parents and disables adding/removing.)
      d.Discard();

      // Can't add or remove anymore.
      Assert.IsFalse(d.AddParent(parent));
      Assert.IsFalse(d.RemoveParent(parent));
    }

    [TestMethod]
    public void TestDependentAdd() {
      var parent = new DiscardableImpl();
      var d = new DiscardableImpl();

      d.AddParent(parent);

      var output = "";
      d.OnDiscard += _ => output += "foobar";

      // Text should be added on first discard.
      parent.Discard();
      Assert.IsTrue(d.IsDiscarded);
      Assert.AreEqual("foobar", output);

      // Nothing should happen on successive discards.
      output = "";
      Assert.IsFalse(d.Discard());
      Assert.AreEqual("", output);
    }

    [TestMethod]
    public void TestDependentRemove() {
      var parent = new DiscardableImpl();
      var d = new DiscardableImpl();

      d.AddParent(parent);
      d.RemoveParent(parent);

      var output = "";
      d.OnDiscard += _ => output += "foobar";

      // Nothing should happen since it was removed.
      parent.Discard();
      Assert.IsFalse(d.IsDiscarded);
      Assert.AreEqual("", output);
    }

    [TestMethod]
    public void TestDiscardAtEndOfScope() {
      var output = "";

      {
        var d = new DiscardableImpl();
        d.OnDiscard += _ => output += "foobar";
      }

      // Nothing should happen since it was removed.
      Assert.AreEqual("foobar", output);
    }
  }
}