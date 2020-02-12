using System;
using System.Collections.Generic;

using fin.data.collections.set;
using fin.pointer.contract.impl;

namespace fin.pointer.contract {

  /// <summary>
  ///   Do not inherit this type directly.
  /// </summary>
  public interface IContract {
    bool IsActive { get; }

    delegate void OnBreakHandler(IContract contract);

    event OnBreakHandler OnBreak;

    bool Break();
  }

  /// <summary>
  ///   Do not inherit this type directly.
  /// </summary>
  public interface IContractPointer<T> : IContract {
    T Value { get; }
  }

  /// <summary>
  ///   Interface for a closed contract pointer. Once instantiated, no new
  ///   owners can join this contract. Preferred over open contracts.
  /// </summary>
  public interface IClosedContractPointer<T> : IContractPointer<T> {
  }

  /// <summary>
  ///   Interface for an open contract pointer. Even after instantiation, new
  ///   owners can join this contract. Prefer a closed contract if possible.
  /// </summary>
  public interface IOpenContractPointer<T> : IContractPointer<T> {

    bool Join(IContractPointerOwner<T> owner);
  }

  /// <summary>
  ///   Interface for a special contract that is a combination of other
  ///   contracts. Once this contract becomes empty or all of the contracts
  ///   contained within are broken, this will also break.
  /// </summary>
  public interface ISuperContract : IContract {

    bool Add(IContract contract);

    bool Remove(IContract contract);
  }

  /// <summary>
  ///   Do not inherit this type directly.
  /// </summary>
  public interface IContractPointerOwner<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }

    IOpenContractPointer<T> FormOpen(T value);

    IOpenContractPointer<T> FormOpenWith(T value, IContractPointerOwner<T> other, params IContractPointerOwner<T>[] additional);

    IClosedContractPointer<T> FormClosedWith(T value, IContractPointerOwner<T> other, params IContractPointerOwner<T>[] additional);

    bool Join(IOpenContractPointer<T> contract);

    bool Break(IContractPointer<T> contract);

    void BreakAll();
  }

  public interface IStrongContractPointerOwner<T> : IContractPointerOwner<T> {
  }

  public interface IWeakContractPointerOwner<T> : IContractPointerOwner<T> {
  }

  /// <summary>
  ///   Interface for the underlying storage of an IContractPointerOwner. This
  ///   type shouldn't perform any joining/breaking, just storing data.
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
    private OrderedSet<IContractPointer<T>> impl_ = new OrderedSet<IContractPointer<T>>();

    public IEnumerable<IContractPointer<T>> Contracts => this.impl_;

    public int Count => this.impl_.Count;

    public bool Add(IContractPointer<T> contract) => this.impl_.Add(contract);

    public bool Remove(IContractPointer<T> contract) => this.impl_.Remove(contract);

    public void ClearAndBreak(Action<IContractPointer<T>> breakHandler) {
      while (this.impl_.Count > 0) {
        breakHandler(this.impl_.First);
      }
    }
  }

  /// <summary>
  ///   Interface for creating contract-related instances. These must be
  ///   guaranteed to be compatible with each other. Compatibility is not
  ///   guaranteed with instances from other factories.
  /// </summary>
  public interface IContractFactory {
    protected readonly static IDelegator<IContractFactory> DELEGATED_INSTANCE = new Delegator<IContractFactory>();

    public static IContractFactory Instance { get; } = new ContractFactory();

    ISuperContract NewSuperContract(IContract first, params IContract[] additional);

    IStrongContractPointerOwner<T> NewStrongOwner<T>();

    IStrongContractPointerOwner<T> NewStrongOwner<T>(IContractPointerSet<T> set);

    IWeakContractPointerOwner<T> NewWeakOwner<T>();

    IWeakContractPointerOwner<T> NewWeakOwner<T>(IContractPointerSet<T> set);
  }
}