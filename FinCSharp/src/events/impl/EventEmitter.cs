namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    public IEventEmitter NewEmitter() => new EventEmitter();

    private class EventEmitter : EventSource, IEventEmitter {

      public void Emit(EventType eventType) {
        var contracts = this.Get(eventType);
        if (contracts != null) {
          foreach (var contract in contracts) {
            var genericSubscription = contract.Value;
            var subscription = genericSubscription as IEventSubscriptionVoid;
            subscription!.Handler();
          }
        }
      }

      public void Emit<T>(EventType<T> eventType, T value) {
        var contracts = this.Get(eventType);
        if (contracts != null) {
          foreach (var contract in contracts) {
            var genericSubscription = contract.Value;
            var subscription = genericSubscription as IEventSubscription<T>;
            subscription!.Handler(value);
          }
        }
      }
    }
  }
}