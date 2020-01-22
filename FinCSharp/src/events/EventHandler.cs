using System;

namespace fin.events {

  public interface IEventListener {

    // TODO: Can we hide this?
    IEventSubscription<T> ListenTo<T>(IEventSubscription<T> subscription);

    // TODO: Can we hide this?
    void UnsubscribeFrom<T>(IEventSubscription<T> subscription);

    void UnsubscribeFromAll();
  }

  public interface IEventHandler : IEventListener {

    IEventSubscription<T> ListenTo<T>(IEventSource source, EventType<T> eventType, Action<T> handler);
  }

  // TODO: Add tests.

  public class EventHandler : IEventHandler {
    private readonly IEventSubscriptionMap subscriptions_ = new EventSubscriptionMap();

    public IEventSubscription<T> ListenTo<T>(IEventSource source, EventType<T> eventType, Action<T> handler) => this.ListenTo(new EventSubscription<T>(source, this, eventType, handler));

    public IEventSubscription<T> ListenTo<T>(IEventSubscription<T> subscription) {
      if (this.subscriptions_.Add(subscription)) {
        subscription.Source.Subscribe(subscription);
      }
      return subscription;
    }

    public void UnsubscribeFrom<T>(IEventSubscription<T> subscription) {
      if (this.subscriptions_.Remove(subscription)) {
        subscription.Unsubscribe();
      }
    }

    // TODO: This will cause a lot of churn, is there a way to optimize this from many Remove() calls to just a Clear()?
    public void UnsubscribeFromAll() {
      var allSubscriptions = this.subscriptions_.AllSubscriptions;
      foreach (var subscription in allSubscriptions) {
        subscription.Unsubscribe();
      }
    }
  }
}