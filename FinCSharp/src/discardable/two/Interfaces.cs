/*using System;

using fin.discardable;

namespace fin.discardabl2 {
  public interface IDiscardableNode {
    bool IsDiscarded { get; }
    bool Discard();

    void SetParent(IDiscardableNode parent);

    /// <summary>
    ///   A "using" statement to tie the lifetime of a disposable to a
    ///   discardable node.
    /// </summary>
    void Using(IDisposable child);
  }

  public interface IDiscardableNodeFactory {
    IDiscardableNode Root { get; }

    IDiscardableNode CreateChild(IDiscardableNode parent);
  }

  public sealed class DiscardableNodeFactoryImpl : IDiscardableNodeFactory {
    public IDiscardableNode Root { get; } = new DiscardableNodeImpl(null);

    public IDiscardableNode CreateChild(IDiscardableNode parent)
      => new DiscardableNodeImpl(parent);

    private class DiscardableNodeImpl : IDiscardableNode {
      public DiscardableNodeImpl(IDiscardableNode? parent) {}
      public bool IsDiscarded { get; private set; }

      public void SetParent(IDiscardableNode parent) {

      }

      public void Using(IDisposable child) {

      }
    }
  }
}*/