namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    public IStrongContractOwner<T> NewStrongOwner<T>() => this.NewStrongOwner(new DefaultContractSet<T>());

    public IStrongContractOwner<T> NewStrongOwner<T>(IContractSet<T> set) => new StrongContractOwner<T>(set);

    private class StrongContractOwner<T> : ContractOwnerImpl<T>, IStrongContractOwner<T> {

      public StrongContractOwner(IContractSet<T> set) : base(set) {
      }
    }
  }
}