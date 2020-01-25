using System;
using System.Collections.Concurrent;
using System.Linq;

using fin.pointer.contract;

namespace fin.src.events {

  public interface IEventType { }

  public class EventType<T> : IEventType { }

  public interface IEventSubscription {
    EventSource Source { get; }
    EventListener Listener { get; }

    IEventType IEventType { get; }
    Action<object> Handler { get; }
    bool IsSubscribed { get; }

    void Unsubscribe();
  }

  public interface IEventSubscription<T> : IEventSubscription {
    EventType<T> EventType { get; }
  }

  public abstract class ContractEventOwner {

    private class ContractEventSubscription<T> : IEventSubscription<T> {
      public ContractPointer<IEventSubscription>? Contract { get; set; }

      public EventSource Source { get; }

      public EventListener Listener { get; }

      public IEventType IEventType => this.EventType;

      public EventType<T> EventType { get; }
      public Action<object> Handler { get; }

      public ContractEventSubscription(EventSource source, EventListener listener, EventType<T> eventType, Action<object> handler) {
        this.Source = source;
        this.Listener = listener;
        this.EventType = eventType;
        this.Handler = handler;
      }

      public bool IsSubscribed { get; private set; } = true;

      public void Unsubscribe() {
        if (this.IsSubscribed) {
          this.Contract!.Break();
          this.IsSubscribed = false;
        }
      }
    }

    private class ContractEventSubscriptionDictionary : IContractOwner<IEventSubscription> {

      // TODO: Is there a safer way to do this...
      private readonly ConcurrentDictionary<object, object> contractSets_ =
        new ConcurrentDictionary<object, object>();

      public IContractSet<IEventSubscription> Get(IEventType eventType) {
        var genericContractSet = this.contractSets_.GetOrAdd(eventType, eventType => new ContractSet<IEventSubscription>());
        var contractSet = genericContractSet as IContractSet<IEventSubscription>;
        return contractSet!;
      }

      public bool Join(IContractPointer<IEventSubscription> genericContract)
        => this.Get(genericContract.Value.IEventType).Join(genericContract);

      public bool Break(IContractPointer<IEventSubscription> contract) {
        var genericEventType = contract.Value.IEventType;
        if (this.contractSets_.TryGetValue(genericEventType, out object? genericContractSet)) {
          var contractSet = genericContractSet as ContractSet<IEventSubscription>;
          if (contractSet!.Break(contract)) {
            if (contractSet.Contracts.Count() == 0) {
              this.contractSets_.TryRemove(genericEventType, out object? _);
            }
            return true;
          }
        }
        return false;
      }

      public void BreakAll() {
        foreach (var genericContractSet in this.contractSets_.Values) {
          var contractSet = genericContractSet as ContractSet<IEventSubscription>;
          contractSet!.BreakAll();
        }
        this.contractSets_.Clear();
      }
    }

    private ContractEventSubscriptionDictionary contractSets_ = new ContractEventSubscriptionDictionary();

    protected IContractSet<IEventSubscription> Get(IEventType eventType)
      => this.contractSets_.Get(eventType);

    protected static ContractPointer<IEventSubscription> CreateContract<T>(EventSource source, EventListener listener, EventType<T> eventType, Action<object> action) {
      var subscription = new ContractEventSubscription<T>(source, listener, eventType, action);
      var contract =
        new ContractPointer<IEventSubscription>(subscription, new[] { source.contractSets_, listener.contractSets_, });
      subscription.Contract = contract;
      return contract!;
    }

    public void UnsubscribeAll() {
      this.contractSets_.BreakAll();
    }
  }

  public abstract class EventSource : ContractEventOwner {

    public IEventSubscription Subscribe<T>(EventListener listener, EventType<T> eventType, Action<T> action) =>
      // TODO: Figure out how to do this without the wrapper.
      ContractEventOwner.CreateContract(this, listener, eventType, (object o) => action((T)o)).Value;
  }

  public class EventEmitter : EventSource {

    public void Emit<T>(EventType<T> eventType, T value) where T : notnull {
      var contracts = this.Get(eventType).Contracts;
      foreach (var contract in contracts) {
        contract.Value.Handler(value);
      }
    }
  }

  public class EventListener : ContractEventOwner {
  }
}