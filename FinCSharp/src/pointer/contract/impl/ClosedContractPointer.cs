using System;
using System.Collections.Generic;
using System.Text;

namespace fin.pointer.contract.impl {

  public partial class ContractFactory : IContractFactory {

    // TODO: Add tests.
    private class ClosedContractPointer<T> : ContractPointerImpl<T>, IClosedContractPointer<T> {

      public ClosedContractPointer(T value, params IContractOwner<T>[] owners) : base(value, owners) {
      }
    }
  }
}