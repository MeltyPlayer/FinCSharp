﻿using System;
using fin.pointer.contract;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    private class EventSubscription : IEventSubscriptionVoid {
      public IClosedContractPointer<IEventSubscription>? Contract { get; set; }

      public IEventSource Source { get; }

      public IEventListener Listener { get; }

      public IEventType IEventType => this.EventType;

      public EventType EventType { get; }
      public Action Handler { get; }

      public EventSubscription(IEventSource source, IEventListener listener, EventType eventType, Action handler) {
        this.Source = source;
        this.Listener = listener;
        this.EventType = eventType;
        this.Handler = handler;
      }

      public bool IsSubscribed => this.Contract!.IsActive;

      public bool Unsubscribe() => this.Contract!.Break();
    }
  }
}