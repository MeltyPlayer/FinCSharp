using fin.type;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {
    public IEventEmitter NewEmitter() => new EventEmitter();

    private class EventEmitter : EventSource, IEventEmitter {
      public void Emit(Event evt) {
        var contracts = this.Get(evt.GenericSafeType);
        if (contracts != null) {
          foreach (var contract in contracts) {
            var genericSubscription = contract.Value;
            var subscription = genericSubscription as IEventSubscriptionVoid;
            subscription!.Handler(evt);
          }
        }
      }

      public void Emit<T>(Event<T> evt, T value) {
        var contracts = this.Get(evt.GenericSafeType);
        if (contracts != null) {
          foreach (var contract in contracts) {
            var genericSubscription = contract.Value;
            var subscription = genericSubscription as IEventSubscription<T>;
            subscription!.Handler(evt, value);
          }
        }
      }
    }
  }
}