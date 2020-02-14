using System;
using fin.type;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    private abstract class EventSource : EventOwner, IEventSource {
      public IEventSubscriptionVoid AddListener(IEventListener listener, SafeType<Event> eventType, Action<Event> action) =>
        this.CreateSubscription(this, (listener as EventListener)!, eventType, action);

      public IEventSubscription<T> AddListener<T>(IEventListener listener, SafeType<Event<T>> eventType, Action<Event<T>, T> action) =>
        this.CreateSubscription(this, (listener as EventListener)!, eventType, action);

      public void RemoveAllListeners() => this.owner_.BreakAll();
    }
  }
}