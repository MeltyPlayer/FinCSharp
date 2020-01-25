using System.Collections.Generic;
using fin.data.collections;
using fin.data.collections.dictionary;
using fin.pointer.contract;

namespace fin.events {

  public interface IEventSubscriptionMap {
    IEnumerable<IEventSubscription> AllSubscriptions { get; }

    void Clear();

    bool Contains<T>(IEventSubscription<T> subscription);

    IEnumerable<IEventSubscription> Get(IEventListener listener);

    IEnumerable<IEventSubscription<T>> Get<T>(EventType<T> eventType);

    bool Add<T>(IEventSubscription<T> subscription);

    bool Remove<T>(IEventSubscription<T> subscription);
  }

  // TODO: Add tests.
  // TODO: Clean this up. Is it possible to pull out the generic logic into a separate class?
  // TODO: Fairly memory intensive. Is there a way to optimize this?
  public class EventSubscriptionMap : IEventSubscriptionMap {
    private readonly ISet<IEventSubscription<object>> subscriptions_ = new HashSet<IEventSubscription<object>>();
    private readonly IMultiDictionary<IEventListener, IEventSubscription<object>> listenerToSubscriptions_ = new MultiDictionary<IEventListener, IEventSubscription<object>>();
    private readonly IMultiDictionary<EventType<object>, IEventSubscription<object>> eventTypeToSubscriptions_ = new MultiDictionary<EventType<object>, IEventSubscription<object>>();

    public IEnumerable<IEventSubscription> AllSubscriptions => this.subscriptions_;

    public void Clear() {
      this.subscriptions_.Clear();
      this.listenerToSubscriptions_.Clear();
      this.eventTypeToSubscriptions_.Clear();
    }

    public bool Contains<T>(IEventSubscription<T> subscription) {
      var genericSubscription = (subscription as IEventSubscription<object>)!;
      return this.subscriptions_.Contains(genericSubscription!);
    }

    public IEnumerable<IEventSubscription> Get(IEventListener listener) => this.listenerToSubscriptions_.Get(listener);

    public IEnumerable<IEventSubscription<T>> Get<T>(EventType<T> eventType) {
      var genericEventType = eventType as EventType<object>;
      var genericSubscriptions = this.eventTypeToSubscriptions_.Get(genericEventType!);
      var subscriptions = genericSubscriptions as IEnumerable<IEventSubscription<T>>;
      return subscriptions!;
    }

    public bool Add<T>(IEventSubscription<T> subscription) {
      if (!this.Contains(subscription)) {
        var genericSubscription = (subscription as IEventSubscription<object>)!;
        this.subscriptions_.Add(genericSubscription);
        this.listenerToSubscriptions_.Add(genericSubscription.Listener, genericSubscription);
        this.eventTypeToSubscriptions_.Add(genericSubscription.EventType, genericSubscription);
        return true;
      }
      return false;
    }

    public bool Remove<T>(IEventSubscription<T> subscription) {
      if (this.Contains(subscription)) {
        var genericSubscription = (subscription as IEventSubscription<object>)!;
        this.subscriptions_.Remove(genericSubscription);
        this.listenerToSubscriptions_.Remove(genericSubscription.Listener, genericSubscription);
        this.eventTypeToSubscriptions_.Remove(genericSubscription.EventType, genericSubscription);
        return true;
      }
      return false;
    }
  }
}