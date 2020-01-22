using System;
using System.Collections.Generic;
using fin.data.collections;

namespace fin.events {

  public interface IEventSource {

    IEventSubscription<T> Subscribe<T>(IEventListener listener, EventType<T> eventType, Action<T> handler);

    // TODO: Can we hide this?
    IEventSubscription<T> Subscribe<T>(IEventSubscription<T> subscription);

    // TODO: Can we hide this?
    void Unsubscribe<T>(IEventSubscription<T> subscription);

    void UnsubscribeAll();
  }

  public interface IEventEmitter : IEventSource {

    void Emit<T>(EventType<T> eventType, T value);
  }

  // TODO: Add tests.
  public class EventEmitter : IEventEmitter {
    private readonly IEventSubscriptionMap subscriptions_ = new EventSubscriptionMap();

    public IEventSubscription<T> Subscribe<T>(IEventListener listener, EventType<T> eventType, Action<T> handler) => this.Subscribe(new EventSubscription<T>(this, listener, eventType, handler));

    public IEventSubscription<T> Subscribe<T>(IEventSubscription<T> subscription) {
      if (this.subscriptions_.Add(subscription)) {
        //subscription.Listener.SubscribeTo(subscription);
      }
      return subscription;
    }

    public void Unsubscribe<T>(IEventSubscription<T> subscription) {
      if (this.subscriptions_.Remove(subscription)) {
        subscription.Unsubscribe();
      }
    }

    // TODO: This will cause a lot of churn, is there a way to optimize this from many Remove() calls to just a Clear()?
    public void UnsubscribeAll() {
      var allSubscriptions = this.subscriptions_.AllSubscriptions;
      foreach (var subscription in allSubscriptions) {
        subscription.Unsubscribe();
      }
    }

    public void Emit<T>(EventType<T> eventType, T value) {
      var affectedSubscriptions = this.subscriptions_.Get(eventType);
      foreach (var affectedSubscription in affectedSubscriptions) {
        affectedSubscription.Handler(value);
      }
    }
  }
}