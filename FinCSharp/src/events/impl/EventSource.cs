using System;
using fin.type;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    private abstract class EventSource : EventOwner, IEventSource {
      public IEventSubscription AddListener<TEvent>(IEventListener listener, SafeType<TEvent> eventType, Action<TEvent> action) where TEvent : IEvent
        => this.CreateSubscription(this, (listener as EventListener)!, eventType, action);

      public void RemoveAllListeners() => this.owner_.BreakAll();
    }
  }
}