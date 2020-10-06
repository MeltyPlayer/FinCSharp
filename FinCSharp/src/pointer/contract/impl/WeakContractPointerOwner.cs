using fin.discardable;

namespace fin.pointer.contract.impl {
  public sealed partial class ContractFactory : IContractFactory {
    public IWeakContractPointerOwner<T> NewWeakOwner<T>(
        IDiscardableNode parentDiscardable)
      => this.NewWeakOwner(parentDiscardable,
                           new DefaultContractPointerSet<T>());

    public IWeakContractPointerOwner<T> NewWeakOwner<T>(
        IDiscardableNode parentDiscardable,
        IContractPointerSet<T> set)
      => new WeakContractPointerOwner<T>(parentDiscardable, set);

    private class WeakContractPointerOwner<T> : ContractPointerOwnerImpl<T>,
                                                IWeakContractPointerOwner<T> {
      public WeakContractPointerOwner(
          IDiscardableNode parentDiscardable,
          IContractPointerSet<T> set) : base(parentDiscardable, set) {}
    }
  }
}