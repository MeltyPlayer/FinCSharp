using System.Collections.Generic;
using System.Linq;

namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    /// <summary>
    ///   Do not inherit from this class directly.
    /// </summary>
    private abstract partial class ContractOwnerImpl<T> : IContractOwner<T> {
      private readonly IContractSet<T> set_;

      public ContractOwnerImpl(IContractSet<T> set) {
        this.set_ = set;
      }

      public IEnumerable<IContractPointer<T>> Contracts => this.set_.Contracts;

      public IOpenContractPointer<T> FormOpen(T value) {
        var contract = new ContractOwnerImpl<T>.OpenContractPointer(value, this);

        return contract;
      }

      public IOpenContractPointer<T> FormOpenWith(T value, IContractOwner<T> other, params IContractOwner<T>[] additional) {
        var owners = new HashSet<IContractOwner<T>> { this, other };
        owners.UnionWith(additional);

        var contract = new ContractOwnerImpl<T>.OpenContractPointer(value, owners.ToArray());

        return contract;
      }

      public IClosedContractPointer<T> FormClosedWith(T value, IContractOwner<T> other, params IContractOwner<T>[] additional) {
        var owners = new HashSet<IContractOwner<T>> { this, other };
        owners.UnionWith(additional);

        var contract = new ContractOwnerImpl<T>.ClosedContractPointer(value, owners.ToArray());

        return contract;
      }

      public bool Join(IOpenContractPointer<T> contract) {
        if (!contract.IsActive) {
          return false;
        }

        var openContract = (contract as ContractOwnerImpl<T>.OpenContractPointer)!;
        if (this.set_.Add(openContract)) {
          contract.Join(this);
          return true;
        }
        return false;
      }

      // TODO: PLEASE find a way to get rid of this.
      private bool JoinBackdoor_(ContractOwnerImpl<T>.ContractPointerImpl contract) {
        if (!contract.IsActive) {
          return false;
        }

        if (this.set_.Add(contract)) {
          return true;
        }
        return false;
      }

      public bool Break(IContractPointer<T> contract) {
        var openContract = (contract as ContractOwnerImpl<T>.ContractPointerImpl)!;
        if (this.set_.Remove(openContract)) {
          (contract as ContractOwnerImpl<T>.ContractPointerImpl)!.BreakWith(this);
          return true;
        }
        return false;
      }

      // TODO: This will cause a lot of churn, is there a way to optimize this from many Remove() calls to just a Clear()?
      public void BreakAll() {
        this.set_.Clear(contract => this.Break(contract));
      }
    }
  }
}