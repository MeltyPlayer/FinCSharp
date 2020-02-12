using System;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    private abstract class EventSource : EventOwner, IEventSource {

      public IEventSubscriptionVoid AddListener(IEventListener listener, EventType eventType, Action action) =>
        this.CreateSubscription(this, (listener as EventListener)!, eventType, action);

      public IEventSubscription<T> AddListener<T>(IEventListener listener, EventType<T> eventType, Action<T> action) =>
        this.CreateSubscription(this, (listener as EventListener)!, eventType, action);

      public void RemoveAllListeners() => this.owner_.BreakAll();
    }
  }
}