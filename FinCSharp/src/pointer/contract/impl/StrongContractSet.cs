namespace fin.pointer.contract.impl {

  public partial class ContractFactory : IContractFactory {

    // TODO: Add tests.
    public IStrongContractSet<T> NewStrongSet<T>() => new StrongContractSet<T>();

    private class StrongContractSet<T> : ContractSetImpl<T>, IStrongContractSet<T> {
    }
  }
}