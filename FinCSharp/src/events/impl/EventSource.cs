using System;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    private abstract class EventSource : ContractEventOwner, IEventSource {

      public IEventSubscriptionVoid Subscribe(IEventListener listener, EventType eventType, Action action) {
        var contract = CreateContract(this, (listener as EventListener)!, eventType, action);
        var genericSubscription = contract.Value;
        var subscription = genericSubscription as IEventSubscriptionVoid;
        return subscription!;
      }

      public IEventSubscription<T> Subscribe<T>(IEventListener listener, EventType<T> eventType, Action<T> action) {
        var contract = CreateContract(this, (listener as EventListener)!, eventType, action);
        var genericSubscription = contract.Value;
        var subscription = genericSubscription as IEventSubscription<T>;
        return subscription!;
      }
    }
  }
}