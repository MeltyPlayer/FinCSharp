using System;
using System.Collections.Generic;
using System.Linq;

using fin.discardable;
using fin.type;

namespace fin.events.impl {
  public sealed partial class EventFactory : IEventFactory {
    public IEventRelay NewRelay(IDiscardableNode parentDiscardable)
      => new EventRelay(parentDiscardable);

    private sealed partial class EventRelay : IEventRelay {
      private readonly ISet<IEventSource> relaySources_ =
          new HashSet<IEventSource>();

      private readonly IDictionary<SafeType<IEvent>, IRelayStream> relayStreams_
          = new Dictionary<SafeType<IEvent>, IRelayStream>();

      private readonly IEventListener listener_;
      private readonly IEventEmitter emitter_;

      public EventRelay(IDiscardableNode parentDiscardable) {
        this.listener_ =
            IEventFactory.INSTANCE.NewListener(parentDiscardable);
        this.emitter_ =
            IEventFactory.INSTANCE.NewEmitter(parentDiscardable);
      }

      public void Destroy() {
        foreach (var genericRelayStream in this.relayStreams_.Values.ToList()) {
          genericRelayStream.Destroy();
        }

        this.relayStreams_.Clear();
        this.relaySources_.Clear();
        this.listener_.UnsubscribeAll();
        this.emitter_.RemoveAllListeners();
      }

      public bool AddRelaySource(IEventSource source) {
        if (this.relaySources_.Add(source)) {
          foreach (var genericRelayStream in this.relayStreams_.Values) {
            genericRelayStream.AddSource(source);
          }

          return true;
        }

        return false;
      }

      public bool RemoveRelaySource(IEventSource source) {
        if (this.relaySources_.Remove(source)) {
          foreach (var relayStream in this.relayStreams_.Values) {
            relayStream.RemoveSource(source);
          }

          return true;
        }

        return false;
      }

      public IEventSubscription AddListener<TEvent>(
          IEventListener listener,
          SafeType<TEvent> eventType,
          Action<TEvent> handler) where TEvent : IEvent {
        var genericEventType = new SafeType<IEvent>(eventType.Value);

        RelayStream<TEvent> relayStream;
        if (this.relayStreams_.TryGetValue(genericEventType,
                                           out IRelayStream? genericRelayStream)
        ) {
          relayStream = (genericRelayStream as RelayStream<TEvent>)!;
        }
        else {
          relayStream = new RelayStream<TEvent>(this, eventType);
          this.relayStreams_.Add(genericEventType, relayStream);
        }

        return relayStream.AddListener(listener, handler);
      }

      public void RemoveAllListeners() => this.emitter_.RemoveAllListeners();

      public void Emit<TEvent>(TEvent evt) where TEvent : IEvent
        => this.emitter_.Emit(evt);

      public Action<TEvent> CompileEmit<TEvent>() where TEvent : IEvent
        => this.emitter_.CompileEmit<TEvent>();
    }
  }
}