namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    public IWeakContractPointerOwner<T> NewWeakOwner<T>() => this.NewWeakOwner(new DefaultContractPointerSet<T>());

    public IWeakContractPointerOwner<T> NewWeakOwner<T>(IContractPointerSet<T> set) => new WeakContractPointerOwner<T>(set);

    private class WeakContractPointerOwner<T> : ContractPointerOwnerImpl<T>, IWeakContractPointerOwner<T> {

      public WeakContractPointerOwner(IContractPointerSet<T> set) : base(set) {
      }
    }
  }
}