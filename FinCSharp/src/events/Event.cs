// TODO: This whole damn file is a war crime.
// TODO: How much overhead does this all introduce?

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using fin.data.collections.set;
using fin.pointer.contract;

namespace fin.events {

  public interface IEventType { }

  public class EventType : IEventType { }

  public class EventType<T> : IEventType { }

  public interface IEventSubscription {
    EventSource Source { get; }
    EventListener Listener { get; }

    IEventType IEventType { get; }

    bool IsSubscribed { get; }

    bool Unsubscribe();
  }

  public interface IEventSubscriptionVoid : IEventSubscription {
    EventType EventType { get; }
    Action Handler { get; }
  }

  public interface IEventSubscription<T> : IEventSubscription {
    EventType<T> EventType { get; }
    Action<T> Handler { get; }
  }

  public abstract class ContractEventOwner {

    private class ContractEventSubscription : IEventSubscriptionVoid {
      public IClosedContractPointer<IEventSubscription>? Contract { get; set; }

      public EventSource Source { get; }

      public EventListener Listener { get; }

      public IEventType IEventType => this.EventType;

      public EventType EventType { get; }
      public Action Handler { get; }

      public ContractEventSubscription(EventSource source, EventListener listener, EventType eventType, Action handler) {
        this.Source = source;
        this.Listener = listener;
        this.EventType = eventType;
        this.Handler = handler;
      }

      public bool IsSubscribed => this.Contract!.IsActive;

      public bool Unsubscribe() => this.Contract!.Break();
    }

    private class ContractEventSubscription<T> : IEventSubscription<T> {
      public IContractPointer<IEventSubscription>? Contract { get; set; }

      public EventSource Source { get; }

      public EventListener Listener { get; }

      public IEventType IEventType => this.EventType;

      public EventType<T> EventType { get; }
      public Action<T> Handler { get; }

      public ContractEventSubscription(EventSource source, EventListener listener, EventType<T> eventType, Action<T> handler) {
        this.Source = source;
        this.Listener = listener;
        this.EventType = eventType;
        this.Handler = handler;
      }

      public bool IsSubscribed => this.Contract!.IsActive;

      public bool Unsubscribe() => this.Contract!.Break();
    }

    private class EventContractDictionary : IContractSet<IEventSubscription> {
      private readonly OrderedSet<IContractPointer<IEventSubscription>> contracts_ = new OrderedSet<IContractPointer<IEventSubscription>>();

      private readonly ConcurrentDictionary<IEventType, ISet<IContractPointer<IEventSubscription>>> sets_ =
        new ConcurrentDictionary<IEventType, ISet<IContractPointer<IEventSubscription>>>();

      public int Count => this.contracts_.Count;

      public IEnumerable<IContractPointer<IEventSubscription>> Contracts => this.contracts_;

      public IEnumerable<IContractPointer<IEventSubscription>>? Get(IEventType eventType) {
        this.sets_.TryGetValue(eventType, out ISet<IContractPointer<IEventSubscription>>? set);
        return set;
      }

      public bool Add(IContractPointer<IEventSubscription> contract) {
        if (this.contracts_.Add(contract)) {
          var genericEventType = contract.Value.IEventType;
          var set = this.sets_.GetOrAdd(genericEventType, genericEventType => new HashSet<IContractPointer<IEventSubscription>>());
          set.Add(contract);
          return true;
        }
        return false;
      }

      public bool Remove(IContractPointer<IEventSubscription> contract) {
        var genericEventType = contract.Value.IEventType;
        if (this.sets_.TryGetValue(genericEventType, out ISet<IContractPointer<IEventSubscription>>? set)) {
          if (set!.Remove(contract)) {
            if (set.Count() == 0) {
              this.sets_.TryRemove(genericEventType, out ISet<IContractPointer<IEventSubscription>>? _);
              this.contracts_.Remove(contract);
            }
            return true;
          }
        }
        return false;
      }

      public void Clear(Action<IContractPointer<IEventSubscription>> breakHandler) {
        while (this.contracts_.Count > 0) {
          breakHandler(this.contracts_.First);
        }
        this.sets_.Clear();
      }
    }

    private EventContractDictionary set_ = new EventContractDictionary();
    private IWeakContractOwner<IEventSubscription> owner_;

    public ContractEventOwner() {
      this.owner_ = IContractFactory.Instance.NewWeakOwner(this.set_);
    }

    public IEnumerable<IContractPointer<IEventSubscription>>? Get(IEventType eventType) => this.set_.Get(eventType);

    protected static IContractPointer<IEventSubscription> CreateContract(EventSource source, EventListener listener, EventType eventType, Action action) {
      var subscription = new ContractEventSubscription(source, listener, eventType, action);
      var contract = source.owner_.FormClosedWith(subscription, listener.owner_);
      subscription.Contract = contract;
      return contract!;
    }

    protected static IContractPointer<IEventSubscription> CreateContract<T>(EventSource source, EventListener listener, EventType<T> eventType, Action<T> action) {
      var subscription = new ContractEventSubscription<T>(source, listener, eventType, action);
      var contract = source.owner_.FormClosedWith(subscription, listener.owner_);
      subscription.Contract = contract;
      return contract!;
    }

    public void UnsubscribeAll() => this.owner_.BreakAll();
  }

  public abstract class EventSource : ContractEventOwner {

    public IEventSubscriptionVoid Subscribe(EventListener listener, EventType eventType, Action action) {
      var contract = CreateContract(this, listener, eventType, action);
      var genericSubscription = contract.Value;
      var subscription = genericSubscription as IEventSubscriptionVoid;
      return subscription!;
    }

    public IEventSubscription<T> Subscribe<T>(EventListener listener, EventType<T> eventType, Action<T> action) {
      var contract = CreateContract(this, listener, eventType, action);
      var genericSubscription = contract.Value;
      var subscription = genericSubscription as IEventSubscription<T>;
      return subscription!;
    }
  }

  public class EventEmitter : EventSource {

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

  public class EventListener : ContractEventOwner {
  }
}