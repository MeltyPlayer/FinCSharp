using System;
using System.Linq;

using fin.app.events;
using fin.assert;
using fin.assert.fluent;
using fin.data.collections.set;
using fin.data.graph;
using fin.discardable;
using fin.events;
using fin.type;

using FluentAssertions;

namespace fin.app.node.impl {
  // TODO: Make this internal.
  public sealed partial class InstantiatorImpl : IInstantiator {
    private readonly IDiscardableNode discardableImpl_;
    private IAppNode? rootAppNode_;

    public InstantiatorImpl(IDiscardableNode parentDiscardable) {
      this.discardableImpl_ = parentDiscardable.CreateChild();
    }

    // TODO: This is messy.
    public IAppNode NewRoot() =>
        this.rootAppNode_ = new AppNodeImpl(this.discardableImpl_);

    public IAppNode NewTopLevelChild(params IComponent[] components) =>
        this.NewChild(this.rootAppNode_!, components);

    public IAppNode NewChild(
        IAppNode parent,
        params IComponent[] components) {
      var child = new AppNodeImpl(Expect.That(parent).AsA<AppNodeImpl>());
      foreach (var component in components) {
        child.AddComponent(component);
      }

      return child;
    }

    public TComponent Wrap<TComponent>(IAppNode parent, TComponent component)
        where TComponent : IComponent {
      var child = new AppNodeImpl(Expect.That(parent).AsA<AppNodeImpl>());
      child.AddComponent(component);
      return component;
    }
  }
}