namespace fin.pointer.contract.impl {
  public sealed partial class ContractFactory {
    private abstract partial class ContractPointerOwnerImpl<T> {
      private class OpenContractPointer : ContractPointerImpl,
                                          IOpenContractPointer<T> {
        public OpenContractPointer(
            T value,
            params IContractPointerOwner<T>[] owners) :
            base(value, owners) {}
      }
    }
  }
}