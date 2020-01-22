using System.Collections.Generic;

namespace fin.pointer.contract {

  public interface IContractPointer<T> {
    T Value { get; }

    bool Join(IContractSet<T> set);

    bool IsBroken { get; }

    bool Break();
  }

  // TODO: Add tests.

  public class ContractPointer<T> : IContractPointer<T> {
    private readonly ISet<IContractSet<T>> sets_ = new HashSet<IContractSet<T>>();

    public ContractPointer(T value, IContractSet<T>[] sets) {
      this.Value = value;
      foreach (var set in sets) {
        this.Join(set);
      }
    }

    public T Value { get; private set; }

    public bool Join(IContractSet<T> set) {
      if (!this.IsBroken && this.sets_.Add(set)) {
        set.Join(this);
        return true;
      }
      return false;
    }

    public bool IsBroken { get; private set; }

    public bool Break() {
      if (!this.IsBroken) {
        this.IsBroken = true;
        foreach (var set in this.sets_) {
          set.Break(this);
        }
        this.sets_.Clear();
        return true;
      }
      return false;
    }
  }

  public interface IContractSet<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }

    bool Join(IContractPointer<T> contract);

    bool Break(IContractPointer<T> contract);

    void BreakAll();
  }

  // TODO: Add tests.

  public class ContractSet<T> : IContractSet<T> {
    private readonly ISet<IContractPointer<T>> contracts_ = new HashSet<IContractPointer<T>>();
    public IEnumerable<IContractPointer<T>> Contracts => this.contracts_;

    public bool Join(IContractPointer<T> contract) {
      if (this.contracts_.Add(contract)) {
        contract.Join(this);
        return true;
      }
      return false;
    }

    public bool Break(IContractPointer<T> contract) {
      if (this.contracts_.Remove(contract)) {
        contract.Break();
        return true;
      }
      return false;
    }

    // TODO: This will cause a lot of churn, is there a way to optimize this from many Remove() calls to just a Clear()?
    public void BreakAll() {
      foreach (var contract in this.contracts_) {
        contract.Break();
      }
      this.contracts_.Clear();
    }
  }
}