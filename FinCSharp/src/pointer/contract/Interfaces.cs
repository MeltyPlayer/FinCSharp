using System.Collections.Generic;
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

  public interface IStrongContractSet<T> : IStrongContractOwner<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }
  }

  public interface IWeakContractSet<T> : IWeakContractOwner<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }
  }

  /// <summary>
  ///   Interface for creating contract-related instances. These must be
  ///   guaranteed to be compatible with each other. Compatibility is not
  ///   guaranteed with instances from other factories.
  /// </summary>
  public interface IContractFactory {
    protected readonly static IDelegator<IContractFactory> DELEGATED_INSTANCE = new Delegator<IContractFactory>();

    public static IContractFactory Instance { get; } = new ContractFactory();

    IStrongContractSet<T> NewStrongSet<T>();

    IWeakContractSet<T> NewWeakSet<T>();
  }
}