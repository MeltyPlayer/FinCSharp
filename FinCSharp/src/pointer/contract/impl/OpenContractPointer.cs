using System.Collections.Generic;
using fin.data.collections.set;

namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    private abstract partial class ContractOwnerImpl<T> : IContractOwner<T> {

      private class OpenContractPointer : ContractOwnerImpl<T>.ContractPointerImpl, IOpenContractPointer<T> {

        public OpenContractPointer(T value, params IContractOwner<T>[] owners) : base(value, owners) {
        }
      }
    }
  }
}