namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    public IStrongContractPointerOwner<T> NewStrongOwner<T>() => this.NewStrongOwner(new DefaultContractPointerSet<T>());

    public IStrongContractPointerOwner<T> NewStrongOwner<T>(IContractPointerSet<T> set) => new StrongContractPointerOwner<T>(set);

    private class StrongContractPointerOwner<T> : ContractPointerOwnerImpl<T>, IStrongContractPointerOwner<T> {

      public StrongContractPointerOwner(IContractPointerSet<T> set) : base(set) {
      }
    }
  }
}