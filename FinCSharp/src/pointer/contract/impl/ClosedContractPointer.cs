namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    private abstract partial class ContractPointerOwnerImpl<T> : IContractPointerOwner<T> {

      private class ClosedContractPointer : ContractPointerImpl, IClosedContractPointer<T> {

        public ClosedContractPointer(T value, params IContractPointerOwner<T>[] owners) : base(value, owners) {
        }
      }
    }
  }
}