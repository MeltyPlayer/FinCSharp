using System;
using System.Collections.Generic;

using fin.data.collections.set;
using fin.discardable;
using fin.events;
using fin.type;

namespace fin.app.node {
  public interface IComponent {}

  public interface IAppNode {
    void SetParent(IAppNode parent);

    // TODO: Should this be limited to IChildAppNode?
    bool AddComponent(IComponent component);
    bool RemoveComponent(IComponent component);

    IEventSubscription OnTick<TEvent>(
        SafeType<TEvent> eventType,
        Action<TEvent> handler)
        where TEvent : IEvent;

    Action<TEvent> CompileEmit<TEvent>() where TEvent : IEvent;
  }

  public interface IInstantiator {
    IAppNode NewRoot();
    IAppNode NewTopLevelChild(params IComponent[] components);
    IAppNode NewChild(IAppNode parent, params IComponent[] components);

    TComponent Wrap<TComponent>(IAppNode parent, TComponent component)
        where TComponent : IComponent;
  }
}