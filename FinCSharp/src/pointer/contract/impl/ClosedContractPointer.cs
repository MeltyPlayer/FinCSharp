namespace fin.pointer.contract.impl {
  public sealed partial class ContractFactory {
    private abstract partial class ContractPointerOwnerImpl<T> {
      private class ClosedContractPointer : ContractPointerImpl,
                                            IClosedContractPointer<T> {
        public ClosedContractPointer(
            T value,
            params IContractPointerOwner<T>[] owners) :
            base(value, owners) {}
      }
    }
  }
}