using System;
using fin.type;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {
    public IEventListener NewListener() => new EventListener();

    private class EventListener : EventOwner, IEventListener {
      public IEventSubscriptionVoid SubscribeTo(IEventSource source, SafeType<Event> eventType, Action<Event> action) =>
        this.CreateSubscription(source, this, eventType, action);

      public IEventSubscription<T> SubscribeTo<T>(IEventSource source, SafeType<Event<T>> eventType, Action<Event<T>, T> action) =>
        this.CreateSubscription(source, this, eventType, action);

      public void UnsubscribeAll() => this.owner_.BreakAll();
    }
  }
}