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
    Action Handler { get; }
  }

  public interface IEventSubscription<T> : IEventSubscription {
    EventType<T> EventType { get; }
    Action<T> Handler { get; }
  }

  public interface IEventListener {
  }

  public interface IEventSource {

    IEventSubscriptionVoid Subscribe(IEventListener listener, EventType eventType, Action action);

    IEventSubscription<T> Subscribe<T>(IEventListener listener, EventType<T> eventType, Action<T> action);

    void UnsubscribeAll();
  }

  public interface IEventEmitter : IEventSource {

    public void Emit(EventType eventType);

    public void Emit<T>(EventType<T> eventType, T value);
  }

  public interface IEventFactory {
    public static IEventFactory Instance { get; } = new EventFactory();

    IEventListener NewListener();

    IEventEmitter NewEmitter();
  }
}