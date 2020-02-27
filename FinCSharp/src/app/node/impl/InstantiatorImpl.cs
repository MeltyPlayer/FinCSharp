using System;
using System.Linq;

using fin.app.events;
using fin.assert;
using fin.data.collections.set;
using fin.data.graph;
using fin.discardable;
using fin.events;
using fin.type;

namespace fin.app.node.impl {
  // TODO: Make this internal.
  public sealed partial class InstantiatorImpl : IInstantiator {
    public IRootAppNode NewRoot() => new RootAppNode();

    private sealed class RootAppNode : BAppNode, IRootAppNode {
      public RootAppNode() : base(null) { }

      public bool Discard() => this.discardableImpl_.Discard();
    }

    public IChildAppNode NewChild(IAppNode parent,
      params IComponent[] components) {
      var child = new ChildAppNode((parent as BAppNode)!);
      foreach (var component in components) {
        child.AddComponent(component);
      }

      return child;
    }

    private sealed class ChildAppNode : BAppNode, IChildAppNode {
      public ChildAppNode(BAppNode parent) : base(parent) { }

      public IAppNode Parent {
        get => this.ParentImpl!;
        set => this.ParentImpl = (value as BAppNode)!;
      }
    }

    public TComponent Wrap<TComponent>(IAppNode parent, TComponent component)
      where TComponent : IComponent {
      var child = new ChildAppNode((parent as BAppNode)!);
      child.AddComponent(component);
      return component;
    }
  }
}