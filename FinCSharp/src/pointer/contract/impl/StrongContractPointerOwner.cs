using fin.discardable;

namespace fin.pointer.contract.impl {
  public sealed partial class ContractFactory {
    public IStrongContractPointerOwner<T> NewStrongOwner<T>(
        IDiscardableNode parentDiscardable)
      => this.NewStrongOwner(parentDiscardable,
                             new DefaultContractPointerSet<T>());

    public IStrongContractPointerOwner<T> NewStrongOwner<T>(
        IDiscardableNode parentDiscardable,
        IContractPointerSet<T> set)
      => new StrongContractPointerOwner<T>(parentDiscardable, set);

    private class StrongContractPointerOwner<T> : ContractPointerOwnerImpl<T>,
                                                  IStrongContractPointerOwner<T
                                                  > {
      public StrongContractPointerOwner(
          IDiscardableNode parentDiscardable,
          IContractPointerSet<T> set) : base(parentDiscardable, set) {}
    }
  }
}