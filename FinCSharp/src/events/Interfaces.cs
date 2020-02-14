using System;

using fin.events.impl;

namespace fin.events {

  public interface IEventType { }

  public class EventType : IEventType { }

  public class EventType<T> : IEventType { }

  public interface IEventSubscription {
    IEventSource Source { get; }
    IEventListener Listener { get; }

    IEventType IEventType { get; }

    bool IsSubscribed { get; }

    bool Unsubscribe();
  }

  public interface IEventSubscriptionVoid : IEventSubscription {
    EventType EventType { get; }
    Action<EventType> Handler { get; }
  }

  public interface IEventSubscription<T> : IEventSubscription {
    EventType<T> EventType { get; }
    Action<EventType<T>, T> Handler { get; }
  }

  public interface IEventListener {

    IEventSubscriptionVoid SubscribeTo(IEventSource source, EventType eventType, Action<EventType> action);

    IEventSubscription<T> SubscribeTo<T>(IEventSource source, EventType<T> eventType, Action<EventType<T>, T> action);

    void UnsubscribeAll();
  }

  public interface IEventSource {

    IEventSubscriptionVoid AddListener(IEventListener listener, EventType eventType, Action<EventType> action);

    IEventSubscription<T> AddListener<T>(IEventListener listener, EventType<T> eventType, Action<EventType<T>, T> action);

    void RemoveAllListeners();
  }

  public interface IEventEmitter : IEventSource {

    void Emit(EventType eventType);

    void Emit<T>(EventType<T> eventType, T value);
  }

  public interface IEventRelay : IEventEmitter {

    void Destroy();

    bool AddRelaySource(IEventSource source);

    bool RemoveRelaySource(IEventSource source);
  }

  public interface IEventFactory {
    public static IEventFactory Instance { get; } = new EventFactory();

    IEventListener NewListener();

    IEventEmitter NewEmitter();

    IEventRelay NewRelay();
  }
}