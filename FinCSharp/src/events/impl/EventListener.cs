using System;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    public IEventListener NewListener() => new EventListener();

    private class EventListener : EventOwner, IEventListener {

      public IEventSubscriptionVoid SubscribeTo(IEventSource source, EventType eventType, Action action) =>
        this.CreateSubscription((source as EventSource)!, this, eventType, action);

      public IEventSubscription<T> SubscribeTo<T>(IEventSource source, EventType<T> eventType, Action<T> action) =>
        this.CreateSubscription((source as EventSource)!, this, eventType, action);

      public void UnsubscribeAll() => this.owner_.BreakAll();
    }
  }
}