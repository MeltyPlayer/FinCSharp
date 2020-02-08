using System;
using System.Collections.Generic;
using fin.data.collections.set;
using fin.pointer.contract.impl;

namespace fin.pointer.contract {

  /// <summary>
  ///   Do not inherit this type directly.
  /// </summary>
  public interface IContractPointer<T> {
    T Value { get; }

    bool IsActive { get; }

    bool Break();
  }

  /// <summary>
  ///   Interface for a closed contract pointer. Once instantiated, no new
  ///   owners can join this contract. Preferred over open contract pointers.
  /// </summary>
  public interface IClosedContractPointer<T> : IContractPointer<T> {
  }

  /// <summary>
  ///   Interface for an open contract pointer. Even after instantiation, new
  ///   owners can join this contract. Prefer a closed contract pointer if
  ///   possible.
  /// </summary>
  public interface IOpenContractPointer<T> : IContractPointer<T> {

    bool Join(IContractOwner<T> owner);
  }

  /// <summary>
  ///   Do not inherit this type directly.
  /// </summary>
  public interface IContractOwner<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }

    IOpenContractPointer<T> FormOpen(T value);

    IOpenContractPointer<T> FormOpenWith(T value, IContractOwner<T> other, params IContractOwner<T>[] additional);

    IClosedContractPointer<T> FormClosedWith(T value, IContractOwner<T> other, params IContractOwner<T>[] additional);

    bool Join(IOpenContractPointer<T> contract);

    bool Break(IContractPointer<T> contract);

    void BreakAll();
  }

  public interface IStrongContractOwner<T> : IContractOwner<T> {
  }

  public interface IWeakContractOwner<T> : IContractOwner<T> {
  }

  /// <summary>
  ///   Interface for the underlying storage of an IContractOwner. This type
  ///   shouldn't perform any joining/breaking, just storing data.
  /// </summary>
  public interface IContractSet<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }
    int Count { get; }

    bool Add(IContractPointer<T> contract);

    bool Remove(IContractPointer<T> contract);

    void Clear(Action<IContractPointer<T>> breakHandler);
  }

  public class DefaultContractSet<T> : IContractSet<T> {
    private OrderedSet<IContractPointer<T>> impl_ = new OrderedSet<IContractPointer<T>>();

    public IEnumerable<IContractPointer<T>> Contracts => this.impl_;

    public int Count => this.impl_.Count;

    public bool Add(IContractPointer<T> contract) => this.impl_.Add(contract);

    public bool Remove(IContractPointer<T> contract) => this.impl_.Remove(contract);

    public void Clear(Action<IContractPointer<T>> breakHandler) {
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

    IStrongContractOwner<T> NewStrongOwner<T>();

    IStrongContractOwner<T> NewStrongOwner<T>(IContractSet<T> set);

    IWeakContractOwner<T> NewWeakOwner<T>();

    IWeakContractOwner<T> NewWeakOwner<T>(IContractSet<T> set);
  }
}