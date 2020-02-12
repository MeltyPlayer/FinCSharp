using System.Collections.Generic;

namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    public ISuperContract NewSuperContract(IContract contract, params IContract[] additional) => new SuperContract(contract, additional);

    private class SuperContract : ISuperContract {
      private readonly ISet<IContract> contracts_ = new HashSet<IContract>();

      public SuperContract(IContract contract, params IContract[] additonal) {
        this.Add(contract);
        foreach (var c in additonal) {
          this.Add(c);
        }

        if (this.contracts_.Count == 0) {
          this.Break();
        }
      }

      public bool IsActive { get; private set; } = true;

      public event IContract.OnBreakHandler OnBreak = delegate { };

      public bool Add(IContract contract) {
        if (!this.IsActive) {
          return false;
        }

        if (this.contracts_.Add(contract)) {
          contract.OnBreak += contract => this.Remove(contract);
          return true;
        }

        return false;
      }

      public bool Remove(IContract contract) {
        if (!this.IsActive) {
          return false;
        }

        if (this.contracts_.Remove(contract)) {
          if (this.contracts_.Count == 0) {
            this.Break();
          }
          return true;
        }

        return false;
      }

      public bool Break() {
        if (!this.IsActive) {
          return false;
        }

        this.IsActive = false;
        foreach (var contract in this.contracts_) {
          contract.Break();
        }
        this.OnBreak(this);

        return true;
      }
    }
  }
}