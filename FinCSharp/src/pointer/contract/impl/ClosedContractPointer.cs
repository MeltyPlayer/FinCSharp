using System;
using System.Collections.Generic;
using System.Text;

namespace fin.pointer.contract.impl {

  public sealed partial class ContractFactory : IContractFactory {

    private abstract partial class ContractOwnerImpl<T> : IContractOwner<T> {

      private class ClosedContractPointer : ContractOwnerImpl<T>.ContractPointerImpl, IClosedContractPointer<T> {

        public ClosedContractPointer(T value, params IContractOwner<T>[] owners) : base(value, owners) {
        }
      }
    }
  }
}