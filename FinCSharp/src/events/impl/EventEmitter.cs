using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory : IEventFactory {
    public IEventEmitter NewEmitter() => new EventEmitter();

    private class EventEmitter : EventSource, IEventEmitter {
      public void Emit<TEvent>(TEvent evt) where TEvent : IEvent {
        var contracts = this.Get(evt.SafeType);
        if (contracts != null) {
          foreach (var contract in contracts) {
            var genericSubscription = contract.Value;
            var subscription = genericSubscription as EventSubscription<TEvent>;
            subscription!.Handler(evt);
          }
        }
      }
    }
  }
}