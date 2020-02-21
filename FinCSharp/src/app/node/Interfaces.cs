using System;
using System.Collections.Generic;
using fin.data.collections.set;
using fin.discardable;
using fin.events;
using fin.type;

namespace fin.app.node {

  public interface IComponent : IDependentDiscardable {
  }

  public abstract class BComponent : IComponent {
    private readonly DiscardableImpl discardableImpl_ = new DiscardableImpl();

    public BComponent() {
      this.discardableImpl_.OnDiscard += _ => this.Discard();
    }

    public bool IsDiscarded => this.discardableImpl_.IsDiscarded;

    // TODO: Come up w/ naming scheme for abstract methods?
    protected abstract void Discard();

    // TODO: Is it possible to remove these...????
    public bool AddParent(IEventDiscardable parent)
      => this.discardableImpl_.AddParent(parent);
    public bool RemoveParent(IEventDiscardable parent)
      => this.discardableImpl_.RemoveParent(parent);
  }

  public interface IRootAppNode : IPubliclyDiscardable, IAppNode {
  }

  public interface IChildAppNode : IAppNode {
    IAppNode Parent { get; set; }

    IEventSubscription OnTick<TEvent>(SafeType<TEvent> eventType, Action<TEvent> handler) where TEvent : IEvent;
  }

  public interface IAppNode : IPubliclyDiscardable, IEventDiscardable {
    // TODO: Should this be limited to IChildAppNode?
    bool AddComponent(IComponent component);
    bool RemoveComponent(IComponent component);

    void Emit<TEvent>(TEvent evt) where TEvent : IEvent;
  }

  public interface IInstantiator {
    IRootAppNode NewRoot();
    IChildAppNode NewChild(IAppNode parent, params IComponent[] components);
    TComponent Wrap<TComponent>(IAppNode parent, TComponent component) where TComponent : IComponent;
  }
}