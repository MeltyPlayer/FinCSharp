using System;

namespace fin.events {

  public class EventRelayer : IEventListener, IEventSource {
    private IEventHandler handler_ = new EventHandler();
    private IEventEmitter emitter_ = new EventEmitter();

    public IEventSubscription<T> ListenTo<T>(IEventSource source, EventType<T> eventType) => this.handler_.ListenTo(new EventSubscription<T>(source, this, eventType, value => this.emitter_.Emit(eventType, value)));

    // IEventListener.
    public IEventSubscription<T> ListenTo<T>(IEventSubscription<T> subscription) => this.handler_.ListenTo(subscription);

    public void UnsubscribeFrom<T>(IEventSubscription<T> subscription) => this.handler_.UnsubscribeFrom(subscription);

    public void UnsubscribeFromAll() => this.handler_.UnsubscribeFromAll();

    // IEventSource.
    public IEventSubscription<T> Subscribe<T>(IEventListener listener, EventType<T> eventType, Action<T> handler) => this.emitter_.Subscribe(new EventSubscription<T>(this, listener, eventType, handler));

    public IEventSubscription<T> Subscribe<T>(IEventSubscription<T> subscription) => this.emitter_.Subscribe(subscription);

    public void Unsubscribe<T>(IEventSubscription<T> subscription) => this.emitter_.Unsubscribe(subscription);

    public void UnsubscribeAll() => this.emitter_.UnsubscribeAll();
  }
}