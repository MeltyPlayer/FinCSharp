using System;
using System.Collections.Generic;

using fin.assert.fluent;

namespace fin.discardable {
  public interface IDiscardableNode {
    public delegate void OnDiscardHandler(IDiscardableNode discardable);

    public event OnDiscardHandler OnDiscard;

    bool IsDiscarded { get; }
    bool Discard();

    void SetParent(IDiscardableNode parent);

    IDiscardableNode CreateChild();

    /// <summary>
    ///   A "using" statement to tie the lifetime of a disposable to a
    ///   discardable node.
    /// </summary>
    void Using(IDisposable child);
  }

  public sealed class DiscardableNodeFactoryImpl {
    public DiscardableNodeFactoryImpl(Action<IDiscardableNode> rootHandler) {
      var root = new DiscardableNodeImpl(null);
      rootHandler(root);
    }

    private sealed class DiscardableNodeImpl : IDiscardableNode {
      public bool IsDiscarded { get; private set; }

      private DiscardableNodeImpl? parent_;

      private readonly IList<DiscardableNodeImpl> children_ =
          new List<DiscardableNodeImpl>();

      private readonly IList<IDisposable> usings_ = new List<IDisposable>();

      public event IDiscardableNode.OnDiscardHandler OnDiscard = delegate {};

      public DiscardableNodeImpl(IDiscardableNode? parent) {
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
          this.OnDiscard -= (IDiscardableNode.OnDiscardHandler)d;
        }
      }

      public void SetParent(IDiscardableNode parent)
        => this.SetParent_(Expect.That(parent).AsA<DiscardableNodeImpl>());

      // TODO: Check if this results in a loop!!!
      private void SetParent_(DiscardableNodeImpl parent) {
        this.parent_?.children_.Remove(this);
        (this.parent_ = parent).children_.Add(this);
      }

      public IDiscardableNode CreateChild() => new DiscardableNodeImpl(this);

      public void Using(IDisposable aUsing)
        => this.usings_.Add(aUsing);
    }
  }
}