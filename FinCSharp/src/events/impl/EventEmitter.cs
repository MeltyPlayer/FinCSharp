using System;
using System.Collections.Generic;

using fin.data.collections;
using fin.pointer.contract;
using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory {
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

      public Action<TEvent> CompileEmit<TEvent>() where TEvent : IEvent {
        var contracts = this.Get(new SafeType<IEvent>(typeof(TEvent)));

        return evt => {
          foreach (var contract in contracts!) {
            var genericSubscription = contract.Value;
            var subscription = genericSubscription as EventSubscription<TEvent>;
            subscription!.Handler(evt);
          }
        };
      }
    }
  }
}