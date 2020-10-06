using System;
using System.Collections.Generic;
using System.Linq;

using fin.data.collections.set;
using fin.discardable;
using fin.pointer.contract.impl;

namespace fin.pointer.contract {
  /// <summary>
  ///   Do not inherit this type directly.
  /// </summary>
  public interface IContract {
    bool IsActive { get; }

    public delegate void OnBreakHandler(IContract contract);

    public event OnBreakHandler OnBreak;

    bool Break();
  }

  /// <summary>
  ///   Interface for a contract pointer, a transient pointer shared between
  ///   multiple owners that can be used for things like subscriptions. Do not
  ///   inherit this type directly.
  /// </summary>
  public interface IContractPointer<T> : IContract {
    T Value { get; }
  }

  /// <summary>
  ///   Interface for a closed contract pointer. Once instantiated, no new
  ///   owners can join this contract. Preferred over open contracts.
  /// </summary>
  public interface IClosedContractPointer<T> : IContractPointer<T> {}

  /// <summary>
  ///   Interface for an open contract pointer. Even after instantiation, new
  ///   owners can join this contract. Prefer using closed contracts if
  ///   possible.
  /// </summary>
  public interface IOpenContractPointer<T> : IContractPointer<T> {
    bool Join(IContractPointerOwner<T> owner);
  }

  /// <summary>
  ///   Interface for a special contract that is a combination of other
  ///   contracts, similar to Promise.All(). Once this contract becomes empty
  ///   or all of the contracts contained within are broken, this will also
  ///   break.
  /// </summary>
  public interface ISuperContract : IContract {
    bool Add(IContract contract);

    bool Remove(IContract contract);
  }

  /// <summary>
  ///   Interface for an owner of a contract. Do not inherit this type
  ///   directly.
  /// </summary>
  public interface IContractPointerOwner<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }

    IOpenContractPointer<T> FormOpen(T value);

    IOpenContractPointer<T> FormOpenWith(
        T value,
        IContractPointerOwner<T> other,
        params IContractPointerOwner<T>[] additional);

    IClosedContractPointer<T> FormClosedWith(
        T value,
        IContractPointerOwner<T> other,
        params IContractPointerOwner<T>[] additional);

    bool Join(IOpenContractPointer<T> contract);

    bool Break(IContractPointer<T> contract);

    void BreakAll();
  }

  /// <summary>
  ///   Interface for a contract owner whose contracts depend on to live. If
  ///   all strong owners break with a contract, the contract will break for
  ///   any remaining weak owners.
  /// </summary>
  public interface IStrongContractPointerOwner<T> : IContractPointerOwner<T> {}

  /// <summary>
  ///   Interface for a contract owner whose contracts do NOT depend on to
  ///   live. Any number of weak owners can break with a contract without
  ///   breaking it for everyone.
  /// </summary>
  public interface IWeakContractPointerOwner<T> : IContractPointerOwner<T> {}

  /// <summary>
  ///   Interface for the underlying storage of an IContractPointerOwner. This
  ///   is exposed in case any contract owners need special methods of
  ///   storing/accessing contracts (e.g. events which opt to join an existing
  ///   contract instead of creating a new one). This type shouldn't perform
  ///   any joining/breaking, just storing data.
  /// </summary>
  public interface IContractPointerSet<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }
    int Count { get; }

    bool Add(IContractPointer<T> contract);

    bool Remove(IContractPointer<T> contract);

    void ClearAndBreak(Action<IContractPointer<T>> breakHandler);
  }

  public class DefaultContractPointerSet<T> : IContractPointerSet<T> {
    // TODO: Higher overhead than should be necessary.
    private readonly IFinSet<IContractPointer<T>> impl_ =
        new FinHashSet<IContractPointer<T>>();

    public IEnumerable<IContractPointer<T>> Contracts => this.impl_;

    public int Count => this.impl_.Count;

    public bool Add(IContractPointer<T> contract) => this.impl_.Add(contract);

    public bool Remove(IContractPointer<T> contract) =>
        this.impl_.Remove(contract);

    public void ClearAndBreak(Action<IContractPointer<T>> breakHandler) {
      while (this.impl_.Count > 0) {
        breakHandler(this.impl_.First());
      }
    }
  }

  /// <summary>
  ///   Interface for creating contract-related instances. These must be
  ///   guaranteed to be compatible with each other. Compatibility is not
  ///   guaranteed with instances from other factories.
  /// </summary>
  public interface IContractFactory {
    protected readonly static IDelegator<IContractFactory> DELEGATED_INSTANCE =
        new Delegator<IContractFactory>();

    public readonly static IContractFactory INSTANCE = new ContractFactory();

    /// <summary>
    ///   Number of contracts. Debug type for testing.
    /// </summary>
    public int Count { get; set; }

    ISuperContract NewSuperContract(
        IContract first,
        params IContract[] additional);

    IStrongContractPointerOwner<T> NewStrongOwner<T>(
        IDiscardableNode parentDiscardable);

    IStrongContractPointerOwner<T> NewStrongOwner<T>(
        IDiscardableNode parentDiscardable,
        IContractPointerSet<T> set);

    IWeakContractPointerOwner<T> NewWeakOwner<T>(
        IDiscardableNode parentDiscardable);

    IWeakContractPointerOwner<T> NewWeakOwner<T>(
        IDiscardableNode parentDiscardable,
        IContractPointerSet<T> set);
  }
}