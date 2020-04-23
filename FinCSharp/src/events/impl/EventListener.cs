using System;

using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory {
    public IEventListener NewListener() => new EventListener();

    private class EventListener : EventOwner, IEventListener {
      public IEventSubscription SubscribeTo<TEvent>(
          IEventSource source,
          SafeType<TEvent> eventType,
          Action<TEvent> action) where TEvent : IEvent =>
          this.CreateSubscription(source, this, eventType, action);

      public void UnsubscribeAll() => this.owner_.BreakAll();
    }
  }
}