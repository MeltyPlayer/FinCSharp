using System;

using fin.discardable;
using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory {
    public IEventListener NewListener(IDiscardableNode parentDiscardable)
      => new EventListener(parentDiscardable);

    private class EventListener : EventOwner, IEventListener {
      public EventListener(IDiscardableNode parentDiscardable)
          : base(parentDiscardable) {}

      public IEventSubscription SubscribeTo<TEvent>(
          IEventSource source,
          SafeType<TEvent> eventType,
          Action<TEvent> action) where TEvent : IEvent =>
          this.CreateSubscription(source, this, eventType, action);

      public void UnsubscribeAll() => this.owner_.BreakAll();
    }
  }
}