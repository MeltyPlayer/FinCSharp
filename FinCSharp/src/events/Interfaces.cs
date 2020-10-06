using System;

using fin.discardable;
using fin.events.impl;
using fin.type;

namespace fin.events {
  /// <summary>
  ///   Interface for a type of event.
  /// </summary>
  public interface IEvent {
    SafeType<IEvent> SafeType { get; }
  }

  /// <summary>
  ///   Abstract base class for a type of event that takes care of overriding
  ///   SafeType.
  /// </summary>
  public abstract class BEvent : IEvent {
    public SafeType<IEvent> SafeType { get; }

    public BEvent() {
      var type = this.GetType();
      this.SafeType = new SafeType<IEvent>(type);
    }
  }

  /// <summary>
  ///   Interface for a subscription between a source and a listener. This can
  ///   be thought of as a contract, where breaking the contract unsubscribes.
  ///   This can be done manually or automatically (e.g. if either the source
  ///   or listener is destroyed).
  /// </summary>
  public interface IEventSubscription {
    IEventSource Source { get; }
    IEventListener Listener { get; }

    SafeType<IEvent> EventType { get; }

    bool IsSubscribed { get; }

    bool Unsubscribe();
  }

  /// <summary>
  ///   Interface for a listener that can subscribe to event sources.
  /// </summary>
  public interface IEventListener {
    IEventSubscription SubscribeTo<TEvent>(
        IEventSource source,
        SafeType<TEvent> eventType,
        Action<TEvent> action) where TEvent : IEvent;

    void UnsubscribeAll();
  }

  /// <summary>
  ///   Interface for a subscribable event source.
  /// </summary>
  public interface IEventSource {
    IEventSubscription AddListener<TEvent>(
        IEventListener listener,
        SafeType<TEvent> eventType,
        Action<TEvent> action) where TEvent : IEvent;

    void RemoveAllListeners();
  }

  /// <summary>
  ///   Interface for an event source that can be triggered publicly, vs. the
  ///   default which is entirely encapsulated.
  /// </summary>
  public interface IEventEmitter : IEventSource {
    void Emit<TEvent>(TEvent evt) where TEvent : IEvent;

    Action<TEvent> CompileEmit<TEvent>() where TEvent : IEvent;
  }

  /// <summary>
  ///   Interface for an event emitter that can also be triggered both manually
  ///   and automatically by upstream sources (in which case it propagates the
  ///   event).
  /// </summary>
  public interface IEventRelay : IEventEmitter {
    void Destroy();

    bool AddRelaySource(IEventSource source);

    bool RemoveRelaySource(IEventSource source);
  }

  /// <summary>
  ///   Interface for factories that create event instances.
  /// </summary>
  public interface IEventFactory {
    public static readonly IEventFactory INSTANCE = new EventFactory();

    IEventListener NewListener(IDiscardableNode parentDiscardable);

    IEventEmitter NewEmitter(IDiscardableNode parentDiscardable);

    IEventRelay NewRelay(IDiscardableNode parentDiscardable);
  }
}