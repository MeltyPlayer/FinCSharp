using System.Collections.Generic;
using fin.data.collections.set;

namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    private abstract partial class ContractPointerOwnerImpl<T> : IContractPointerOwner<T> {

      private class OpenContractPointer : ContractPointerImpl, IOpenContractPointer<T> {

        public OpenContractPointer(T value, params IContractPointerOwner<T>[] owners) : base(value, owners) {
        }
      }
    }
  }
}