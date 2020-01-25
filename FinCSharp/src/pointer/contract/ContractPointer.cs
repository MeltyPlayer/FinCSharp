using System.Collections.Generic;

namespace fin.pointer.contract {

  public interface IContractPointer<T> {
    T Value { get; }

    bool Join(IContractOwner<T> owner);

    bool IsBroken { get; }

    bool Break();
  }

  // TODO: Add tests.

  public class ContractPointer<T> : IContractPointer<T> {
    private readonly ISet<IContractOwner<T>> owners_ = new HashSet<IContractOwner<T>>();

    public ContractPointer(T value, IContractOwner<T>[] owners) {
      this.Value = value;
      foreach (var owner in owners) {
        this.Join(owner);
      }
    }

    public T Value { get; private set; }

    public bool Join(IContractOwner<T> owner) {
      if (!this.IsBroken && this.owners_.Add(owner)) {
        owner.Join(this);
        return true;
      }
      return false;
    }

    public bool IsBroken { get; private set; }

    public bool Break() {
      if (!this.IsBroken) {
        this.IsBroken = true;
        foreach (var owner in this.owners_) {
          owner.Break(this);
        }
        this.owners_.Clear();
        return true;
      }
      return false;
    }
  }

  public interface IContractOwner<T> {

    bool Join(IContractPointer<T> contract);

    bool Break(IContractPointer<T> contract);

    void BreakAll();
  }

  public interface IContractSet<T> : IContractOwner<T> {
    IEnumerable<IContractPointer<T>> Contracts { get; }
  }

  // TODO: Add tests.

  public class ContractSet<T> : IContractSet<T> {
    private readonly LinkedList<IContractPointer<T>> contracts_ = new LinkedList<IContractPointer<T>>();
    public IEnumerable<IContractPointer<T>> Contracts => this.contracts_;

    public bool Join(IContractPointer<T> contract) {
      if (!this.contracts_.Contains(contract)) {
        this.contracts_.AddLast(contract);
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
      while (this.contracts_.Count > 0) {
        var contract = this.contracts_.First!.Value;
        contract.Break();
        // Break removes the node from the list.
      }
    }
  }
}