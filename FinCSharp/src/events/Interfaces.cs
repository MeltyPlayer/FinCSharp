using System;

using fin.events.impl;
using fin.type;

namespace fin.events {
  public interface IEvent {
    SafeType<IEvent> SafeType { get; }
  }

  public abstract class BEvent : IEvent {
    public SafeType<IEvent> SafeType { get; }

    public BEvent() {
      var type = this.GetType();
      this.SafeType = new SafeType<IEvent>(type);
    }
  }

  public interface IEventSubscription {
    IEventSource Source { get; }
    IEventListener Listener { get; }

    SafeType<IEvent> EventType { get; }

    bool IsSubscribed { get; }

    bool Unsubscribe();
  }

  public interface IEventListener {
    IEventSubscription SubscribeTo<TEvent>(IEventSource source,
      SafeType<TEvent> eventType,
      Action<TEvent> action) where TEvent : IEvent;

    void UnsubscribeAll();
  }

  public interface IEventSource {
    IEventSubscription AddListener<TEvent>(IEventListener listener,
      SafeType<TEvent> eventType,
      Action<TEvent> action) where TEvent : IEvent;

    void RemoveAllListeners();
  }

  public interface IEventEmitter : IEventSource {
    void Emit<TEvent>(TEvent evt) where TEvent : IEvent;
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