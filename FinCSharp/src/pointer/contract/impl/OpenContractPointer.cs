using System.Collections.Generic;
using fin.data.collections.set;

namespace fin.pointer.contract.impl {

  public partial class ContractFactory : IContractFactory {

    private class OpenContractPointer<T> : ContractPointerImpl<T>, IOpenContractPointer<T> {

      public OpenContractPointer(T value, params IContractOwner<T>[] owners) : base(value, owners) {
      }
    }
  }
}