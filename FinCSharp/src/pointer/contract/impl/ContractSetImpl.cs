using System.Collections.Generic;
using System.Linq;
using fin.data.collections.set;

namespace fin.pointer.contract.impl {

  public partial class ContractFactory : IContractFactory {
    // TODO: Add tests.

    private abstract class ContractSetImpl<T> : IContractOwner<T> {
      private readonly OrderedSet<ContractPointerImpl<T>> contracts_ = new OrderedSet<ContractPointerImpl<T>>();
      public IEnumerable<IContractPointer<T>> Contracts => this.contracts_;

      public IOpenContractPointer<T> FormOpen(T value) {
        var contract = new OpenContractPointer<T>(value, this);

        return contract;
      }

      public IOpenContractPointer<T> FormOpenWith(T value, IContractOwner<T> other, params IContractOwner<T>[] additional) {
        var owners = new HashSet<IContractOwner<T>> { this, other };
        owners.UnionWith(additional);

        var contract = new OpenContractPointer<T>(value, owners.ToArray());

        return contract;
      }

      public IClosedContractPointer<T> FormClosedWith(T value, IContractOwner<T> other, params IContractOwner<T>[] additional) {
        var owners = new HashSet<IContractOwner<T>> { this, other };
        owners.UnionWith(additional);

        var contract = new ClosedContractPointer<T>(value, owners.ToArray());

        return contract;
      }

      public bool Join(IOpenContractPointer<T> contract) {
        if (!contract.IsActive) {
          return false;
        }

        var openContract = (contract as OpenContractPointer<T>)!;
        if (this.contracts_.Add(openContract)) {
          contract.Join(this);
          return true;
        }
        return false;
      }

      // TODO: PLEASE find a way to get rid of this.
      public bool JoinBackdoor(ContractPointerImpl<T> contract) {
        if (!contract.IsActive) {
          return false;
        }

        return this.contracts_.Add(contract);
      }

      public bool Break(IContractPointer<T> contract) {
        var openContract = (contract as ContractPointerImpl<T>)!;
        if (this.contracts_.Remove(openContract)) {
          (contract as ContractPointerImpl<T>)!.BreakWith(this);
          return true;
        }
        return false;
      }

      // TODO: This will cause a lot of churn, is there a way to optimize this from many Remove() calls to just a Clear()?
      public void BreakAll() {
        while (this.contracts_.Count > 0) {
          var contract = this.contracts_.First!;
          contract.Break();
          // Break removes the node from the list.
        }
      }
    }
  }
}