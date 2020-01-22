using System;

namespace fin.events {

  public interface IEventSubscription {
    IEventSource Source { get; }
    IEventListener Listener { get; }

    bool IsSubscribed { get; }

    void Unsubscribe();
  }

  public interface IEventSubscription<T> : IEventSubscription {
    EventType<T> EventType { get; }
    Action<T> Handler { get; }
  }

  public class EventSubscription<T> : IEventSubscription<T> {
    public IEventSource Source { get; }

    public IEventListener Listener { get; }

    public EventType<T> EventType { get; }
    public Action<T> Handler { get; }

    public EventSubscription(IEventSource source, IEventListener listener, EventType<T> eventType, Action<T> handler) {
      this.Source = source;
      this.Listener = listener;
      this.EventType = eventType;
      this.Handler = handler;
    }

    public bool IsSubscribed { get; private set; } = true;

    public void Unsubscribe() {
      if (this.IsSubscribed) {
        this.Source.Unsubscribe(this);
        this.Listener.UnsubscribeFrom(this);
        this.IsSubscribed = false;
      }
    }
  }
}