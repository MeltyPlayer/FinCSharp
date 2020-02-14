using System;

using fin.events.impl;
using fin.type;

namespace fin.events {

  public interface IEvent { }

  public class Event : IEvent { }

  public class Event<T> : IEvent { }

  public interface IEventSubscription {
    IEventSource Source { get; }
    IEventListener Listener { get; }

    SafeType<IEvent> GenericEventType { get; }

    bool IsSubscribed { get; }

    bool Unsubscribe();
  }

  public interface IEventSubscriptionVoid : IEventSubscription {
    SafeType<Event> EventType { get; }
    Action<Event> Handler { get; }
  }

  public interface IEventSubscription<T> : IEventSubscription {
    SafeType<Event<T>> EventType { get; }
    Action<Event<T>, T> Handler { get; }
  }

  public interface IEventListener {
    IEventSubscriptionVoid SubscribeTo(IEventSource source, SafeType<Event> eventType, Action<Event> action);

    IEventSubscription<T> SubscribeTo<T>(IEventSource source, SafeType<Event<T>> eventType, Action<Event<T>, T> action);

    void UnsubscribeAll();
  }

  public interface IEventSource {
    IEventSubscriptionVoid AddListener(IEventListener listener, SafeType<Event> eventType, Action<Event> action);

    IEventSubscription<T> AddListener<T>(IEventListener listener, SafeType<Event<T>> eventType, Action<Event<T>, T> action);

    void RemoveAllListeners();
  }

  public interface IEventEmitter : IEventSource {
    // TODO: This is a bit redundant... can we fix this?
    void Emit(SafeType<Event> eventType, Event evt);

    void Emit<T>(SafeType<Event<T>> eventType, Event<T> evt, T value);
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