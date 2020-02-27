using System;

using fin.pointer.contract;
using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory : IEventFactory {
    private class EventSubscription<TEvent> : IEventSubscription {
      public IClosedContractPointer<IEventSubscription>? Contract { get; set; }

      public IEventSource Source { get; }
      public IEventListener Listener { get; }
      public SafeType<IEvent> EventType { get; }
      public Action<TEvent> Handler { get; }

      public EventSubscription(IEventSource source,
        IEventListener listener,
        SafeType<TEvent> eventType,
        Action<TEvent> handler) {
        this.Source = source;
        this.Listener = listener;
        this.EventType = new SafeType<IEvent>(eventType.Value);
        this.Handler = handler;
      }

      public bool IsSubscribed => this.Contract!.IsActive;
      public bool Unsubscribe() => this.Contract!.Break();
    }
  }
}