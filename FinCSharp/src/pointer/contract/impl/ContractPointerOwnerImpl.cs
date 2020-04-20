using System.Collections.Generic;
using System.Linq;

namespace fin.pointer.contract.impl {
  public sealed partial class ContractFactory {
    private abstract partial class
      ContractPointerOwnerImpl<T> : IContractPointerOwner<T> {
      private readonly IContractPointerSet<T> set_;

      public ContractPointerOwnerImpl(IContractPointerSet<T> set) {
        this.set_ = set;
      }

      public IEnumerable<IContractPointer<T>> Contracts => this.set_.Contracts;

      public IOpenContractPointer<T> FormOpen(T value) {
        var contract = new OpenContractPointer(value, this);

        return contract;
      }

      public IOpenContractPointer<T> FormOpenWith(T value,
        IContractPointerOwner<T> other,
        params IContractPointerOwner<T>[] additional) {
        var owners = new HashSet<IContractPointerOwner<T>> {this, other};
        owners.UnionWith(additional);

        var contract =
          new ContractPointerOwnerImpl<T>.OpenContractPointer(value,
            owners.ToArray());

        return contract;
      }

      public IClosedContractPointer<T> FormClosedWith(T value,
        IContractPointerOwner<T> other,
        params IContractPointerOwner<T>[] additional) {
        var owners = new HashSet<IContractPointerOwner<T>> {this, other};
        owners.UnionWith(additional);

        var contract =
          new ContractPointerOwnerImpl<T>.ClosedContractPointer(value,
            owners.ToArray());

        return contract;
      }

      public bool Join(IOpenContractPointer<T> contract) {
        if (!contract.IsActive) {
          return false;
        }

        var openContract =
          (contract as ContractPointerOwnerImpl<T>.OpenContractPointer)!;
        if (this.set_.Add(openContract)) {
          contract.Join(this);
          return true;
        }

        return false;
      }

      // TODO: PLEASE find a way to get rid of this.
      private bool JoinBackdoor_(
        ContractPointerOwnerImpl<T>.ContractPointerImpl contract) {
        if (!contract.IsActive) {
          return false;
        }

        if (this.set_.Add(contract)) {
          return true;
        }

        return false;
      }

      public bool Break(IContractPointer<T> contract) {
        if (this.set_.Remove(contract)) {
          (contract as ContractPointerOwnerImpl<T>.ContractPointerImpl)!
            .BreakWith(this);
          return true;
        }

        return false;
      }

      // TODO: This will cause a lot of churn, is there a way to optimize this from many Remove() calls to just a Clear()?
      public void BreakAll() {
        this.set_.ClearAndBreak(contract => this.Break(contract));
      }
    }
  }
}