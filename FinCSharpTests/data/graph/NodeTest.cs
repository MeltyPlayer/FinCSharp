using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections;
using System.Linq;

namespace fin.data.graph {

  [TestClass]
  public class NodeTest {
    private readonly ICollection emptyArray_ = Array.Empty<INode<int>>();

    [TestMethod]
    public void TestInitiallyEmpty() {
      var root = new Node<int>(0);

      CollectionAssert.AreEqual(this.emptyArray_, root.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, root.OutgoingNodes.ToArray());
    }

    [TestMethod]
    public void TestAddIncomingSelf() {
      var root = new Node<int>(0);

      Assert.IsTrue(root.AddIncoming(root));
      CollectionAssert.AreEqual(new[] { root }, root.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(new[] { root }, root.OutgoingNodes.ToArray());

      Assert.IsFalse(root.AddIncoming(root));
    }

    [TestMethod]
    public void TestAddOutgoingSelf() {
      var root = new Node<int>(0);

      Assert.IsTrue(root.AddOutgoing(root));
      CollectionAssert.AreEqual(new[] { root }, root.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(new[] { root }, root.OutgoingNodes.ToArray());

      Assert.IsFalse(root.AddOutgoing(root));
    }

    [TestMethod]
    public void TestRemoveIncomingSelf() {
      var root = new Node<int>(0);

      root.AddIncoming(root);

      Assert.IsTrue(root.RemoveIncoming(root));
      CollectionAssert.AreEqual(this.emptyArray_, root.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, root.OutgoingNodes.ToArray());

      Assert.IsFalse(root.RemoveIncoming(root));
    }

    [TestMethod]
    public void TestRemoveOutgoingSelf() {
      var root = new Node<int>(0);

      root.AddOutgoing(root);

      Assert.IsTrue(root.RemoveOutgoing(root));
      CollectionAssert.AreEqual(this.emptyArray_, root.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, root.OutgoingNodes.ToArray());

      Assert.IsFalse(root.RemoveOutgoing(root));
    }

    [TestMethod]
    public void TestAddIncomingOther() {
      var a = new Node<int>(0);
      var b = new Node<int>(1);

      Assert.IsTrue(a.AddIncoming(b));
      CollectionAssert.AreEqual(new[] { b }, a.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, a.OutgoingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(new[] { a }, b.OutgoingNodes.ToArray());

      Assert.IsFalse(a.AddIncoming(b));
    }

    [TestMethod]
    public void TestAddOutgoingOther() {
      var a = new Node<int>(0);
      var b = new Node<int>(1);

      Assert.IsTrue(a.AddOutgoing(b));
      CollectionAssert.AreEqual(this.emptyArray_, a.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(new[] { b }, a.OutgoingNodes.ToArray());
      CollectionAssert.AreEqual(new[] { a }, b.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.OutgoingNodes.ToArray());

      Assert.IsFalse(a.AddOutgoing(b));
    }

    [TestMethod]
    public void TestRemoveIncomingOther() {
      var a = new Node<int>(0);
      var b = new Node<int>(1);

      a.AddIncoming(b);

      Assert.IsTrue(a.RemoveIncoming(b));
      CollectionAssert.AreEqual(this.emptyArray_, a.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, a.OutgoingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.OutgoingNodes.ToArray());

      Assert.IsFalse(a.RemoveIncoming(b));
    }

    [TestMethod]
    public void TestRemoveOutgoingOther() {
      var a = new Node<int>(0);
      var b = new Node<int>(1);

      a.AddOutgoing(b);

      Assert.IsTrue(a.RemoveOutgoing(b));
      CollectionAssert.AreEqual(this.emptyArray_, a.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, a.OutgoingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.IncomingNodes.ToArray());
      CollectionAssert.AreEqual(this.emptyArray_, b.OutgoingNodes.ToArray());

      Assert.IsFalse(a.RemoveOutgoing(b));
    }
  }
}