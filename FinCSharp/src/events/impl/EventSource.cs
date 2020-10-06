using System;

using fin.discardable;
using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory {
    private abstract class EventSource : EventOwner, IEventSource {
      protected EventSource(IDiscardableNode parentDiscardable)
          : base(parentDiscardable) {}

      public IEventSubscription AddListener<TEvent>(
          IEventListener listener,
          SafeType<TEvent> eventType,
          Action<TEvent> action) where TEvent : IEvent
        => this.CreateSubscription(this,
                                   (listener as EventListener)!,
                                   eventType,
                                   action);

      public void RemoveAllListeners() => this.owner_.BreakAll();
    }
  }
}