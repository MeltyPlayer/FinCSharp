namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    public IWeakContractOwner<T> NewWeakOwner<T>() => this.NewWeakOwner(new DefaultContractSet<T>());

    public IWeakContractOwner<T> NewWeakOwner<T>(IContractSet<T> set) => new WeakContractOwner<T>(set);

    private class WeakContractOwner<T> : ContractOwnerImpl<T>, IWeakContractOwner<T> {

      public WeakContractOwner(IContractSet<T> set) : base(set) {
      }
    }
  }
}