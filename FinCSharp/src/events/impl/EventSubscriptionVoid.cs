using System;
using fin.pointer.contract;
using fin.type;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    private class EventSubscription : IEventSubscriptionVoid {
      public IClosedContractPointer<IEventSubscription>? Contract { get; set; }

      public IEventSource Source { get; }
      public IEventListener Listener { get; }
      public SafeType<IEvent> GenericEventType { get; }
      public SafeType<Event> EventType { get; }
      public Action<Event> Handler { get; }

      public EventSubscription(IEventSource source, IEventListener listener, SafeType<Event> eventType, Action<Event> handler) {
        this.Source = source;
        this.Listener = listener;
        this.GenericEventType = new SafeType<IEvent>(eventType.Value);
        this.EventType = eventType;
        this.Handler = handler;
      }

      public bool IsSubscribed => this.Contract!.IsActive;
      public bool Unsubscribe() => this.Contract!.Break();
    }
  }
}