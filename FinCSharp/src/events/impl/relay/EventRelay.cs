using System;
using System.Collections.Generic;
using System.Linq;
using fin.type;

namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {
    public IEventRelay NewRelay() => new EventRelay();

    private sealed partial class EventRelay : IEventRelay {
      private readonly ISet<IEventSource> relaySources_ = new HashSet<IEventSource>();
      private readonly IDictionary<SafeType<Event>, RelayStream> voidRelays_ = new Dictionary<SafeType<Event>, RelayStream>();
      private readonly IDictionary<SafeType<IEvent>, IRelayStream> tRelays_ = new Dictionary<SafeType<IEvent>, IRelayStream>();

      private readonly IEventListener listener_ = IEventFactory.Instance.NewListener();
      private readonly IEventEmitter emitter_ = IEventFactory.Instance.NewEmitter();

      public EventRelay() {
      }

      public void Destroy() {
        foreach (var relay in this.voidRelays_.Values.ToList()) {
          relay.Destroy();
        }
        this.voidRelays_.Clear();
        foreach (var relay in this.tRelays_.Values.ToList()) {
          relay.Destroy();
        }
        this.tRelays_.Clear();
        this.relaySources_.Clear();
        this.listener_.UnsubscribeAll();
        this.emitter_.RemoveAllListeners();
      }

      public bool AddRelaySource(IEventSource source) {
        if (this.relaySources_.Add(source)) {
          foreach (var relay in this.voidRelays_.Values) {
            relay.AddSource(source);
          }
          foreach (var relay in this.tRelays_.Values) {
            relay.AddSource(source);
          }
          return true;
        }
        return false;
      }

      public bool RemoveRelaySource(IEventSource source) {
        if (this.relaySources_.Remove(source)) {
          foreach (var relay in this.voidRelays_.Values) {
            relay.RemoveSource(source);
          }
          foreach (var relay in this.tRelays_.Values) {
            relay.RemoveSource(source);
          }
          return true;
        }
        return false;
      }

      public IEventSubscriptionVoid AddListener(IEventListener listener, SafeType<Event> eventType, Action<Event> handler) {
        if (!this.voidRelays_.TryGetValue(eventType, out RelayStream? relay)) {
          relay = new RelayStream(this, eventType);
          this.voidRelays_.Add(eventType, relay);
        }
        return relay.AddListener(listener, handler);
      }

      public IEventSubscription<T> AddListener<T>(IEventListener listener, SafeType<Event<T>> eventType, Action<Event<T>, T> handler) {
        var genericEventType = new SafeType<IEvent>(eventType.Value);
        RelayStream<T> relay;
        if (this.tRelays_.TryGetValue(genericEventType, out IRelayStream? genericRelay)) {
          relay = (genericRelay as RelayStream<T>)!;
        } else {
          relay = new RelayStream<T>(this, eventType);
          this.tRelays_.Add(genericEventType, relay);
        }
        return relay.AddListener(listener, handler);
      }

      public void RemoveAllListeners() => this.emitter_.RemoveAllListeners();

      public void Emit(Event evt) => this.emitter_.Emit(evt);

      public void Emit<T>(Event<T> evt, T value) => this.emitter_.Emit(evt, value);
    }
  }
}