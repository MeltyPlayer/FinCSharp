using System;
using System.Collections.Generic;

using fin.assert.fluent;

namespace fin.discardable.impl {
  public sealed class DiscardableNodeFactoryImpl {
    public int Count { get; private set; }

    public DiscardableNodeFactoryImpl(Action<IDiscardableNode> rootHandler) {
      var root = new DiscardableNodeImpl(this, null);
      rootHandler(root);
    }

    private sealed class DiscardableNodeImpl : IDiscardableNode {
      public bool IsDiscarded { get; private set; }

      private readonly DiscardableNodeFactoryImpl factory_;
      private DiscardableNodeImpl? parent_;

      private readonly IList<DiscardableNodeImpl> children_ =
          new List<DiscardableNodeImpl>();

      private readonly IList<IDisposable> usings_ = new List<IDisposable>();

      public event IDiscardableNode.OnDiscardHandler OnDiscard = delegate {};

      public DiscardableNodeImpl(
          DiscardableNodeFactoryImpl factory,
          IDiscardableNode? parent) {
        this.factory_ = factory;
        this.factory_.Count++;

        if (parent != null) {
          this.SetParent(parent!);
        }
      }

      public bool Discard() {
        if (this.IsDiscarded) {
          return false;
        }

        this.parent_?.children_.Remove(this);
        this.parent_ = null;
        this.DiscardChildrenAndDisposeUsings_();

        return true;
      }

      private void DiscardChildrenAndDisposeUsings_() {
        this.IsDiscarded = true;
        this.factory_.Count--;

        foreach (var child in this.children_) {
          child.DiscardChildrenAndDisposeUsings_();
        }
        this.children_.Clear();

        foreach (var aUsing in this.usings_) {
          aUsing.Dispose();
        }
        this.usings_.Clear();

        // TODO: Test GC.
        this.OnDiscard.Invoke(this);
        foreach (var d in this.OnDiscard.GetInvocationList()) {
          this.OnDiscard -= (IDiscardableNode.OnDiscardHandler) d;
        }
      }

      public void SetParent(IDiscardableNode parent)
        => this.SetParent_(Expect.That(parent).AsA<DiscardableNodeImpl>());

      // TODO: Check if this results in a loop!!!
      private void SetParent_(DiscardableNodeImpl parent) {
        this.parent_?.children_.Remove(this);
        (this.parent_ = parent).children_.Add(this);
      }

      public IDiscardableNode CreateChild()
        => new DiscardableNodeImpl(this.factory_, this);

      public void Using(IDisposable aUsing)
        => this.usings_.Add(aUsing);
    }
  }
}