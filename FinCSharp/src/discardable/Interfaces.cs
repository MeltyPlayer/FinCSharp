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
}