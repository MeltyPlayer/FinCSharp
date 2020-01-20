using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections;
using System.Linq;

namespace fin.data.tree {

  [TestClass]
  public partial class NodeTest {
    private readonly ICollection emptyArray_ = Array.Empty<INode<int>>();

    [TestInitialize]
    public void SetUp() {
    }

    [TestMethod]
    public void TestInitiallyEmpty() {
      var root = new Node<int>(0);

      CollectionAssert.AreEqual(this.emptyArray_, root.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, root.ToNodes.ToArray());
    }

    [TestMethod]
    public void TestAddFromSelf() {
      var root = new Node<int>(0);

      Assert.IsTrue(root.AddFrom(root));
      CollectionAssert.AreEqual(new[] { root }, root.FromNodes.ToArray());
      CollectionAssert.AreEqual(new[] { root }, root.ToNodes.ToArray());

      Assert.IsFalse(root.AddFrom(root));
    }

    [TestMethod]
    public void TestAddToSelf() {
      var root = new Node<int>(0);

      Assert.IsTrue(root.AddTo(root));
      CollectionAssert.AreEqual(new[] { root }, root.FromNodes.ToArray());
      CollectionAssert.AreEqual(new[] { root }, root.ToNodes.ToArray());

      Assert.IsFalse(root.AddTo(root));
    }

    [TestMethod]
    public void TestRemoveFromSelf() {
      var root = new Node<int>(0);

      root.AddFrom(root);

      Assert.IsTrue(root.RemoveFrom(root));
      CollectionAssert.AreEqual(this.emptyArray_, root.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, root.ToNodes.ToArray());

      Assert.IsFalse(root.RemoveFrom(root));
    }

    [TestMethod]
    public void TestRemoveToSelf() {
      var root = new Node<int>(0);

      root.AddTo(root);

      Assert.IsTrue(root.RemoveTo(root));
      CollectionAssert.AreEqual(this.emptyArray_, root.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, root.ToNodes.ToArray());

      Assert.IsFalse(root.RemoveTo(root));
    }

    [TestMethod]
    public void TestAddFromOther() {
      var a = new Node<int>(0);
      var b = new Node<int>(1);

      Assert.IsTrue(a.AddFrom(b));
      CollectionAssert.AreEqual(new[] { b }, a.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, a.ToNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.FromNodes.ToArray());
      CollectionAssert.AreEqual(new[] { a }, b.ToNodes.ToArray());

      Assert.IsFalse(a.AddFrom(b));
    }

    [TestMethod]
    public void TestAddToOther() {
      var a = new Node<int>(0);
      var b = new Node<int>(1);

      Assert.IsTrue(a.AddTo(b));
      CollectionAssert.AreEqual(this.emptyArray_, a.FromNodes.ToArray());
      CollectionAssert.AreEqual(new[] { b }, a.ToNodes.ToArray());
      CollectionAssert.AreEqual(new[] { a }, b.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.ToNodes.ToArray());

      Assert.IsFalse(a.AddTo(b));
    }

    [TestMethod]
    public void TestRemoveFromOther() {
      var a = new Node<int>(0);
      var b = new Node<int>(1);

      a.AddFrom(b);

      Assert.IsTrue(a.RemoveFrom(b));
      CollectionAssert.AreEqual(this.emptyArray_, a.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, a.ToNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.ToNodes.ToArray());

      Assert.IsFalse(a.RemoveFrom(b));
    }

    [TestMethod]
    public void TestRemoveToOther() {
      var a = new Node<int>(0);
      var b = new Node<int>(1);

      a.AddTo(b);

      Assert.IsTrue(a.RemoveTo(b));
      CollectionAssert.AreEqual(this.emptyArray_, a.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, a.ToNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.FromNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.ToNodes.ToArray());

      Assert.IsFalse(a.RemoveTo(b));
    }
  }
}