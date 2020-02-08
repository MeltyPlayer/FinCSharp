using System.Collections.Generic;

namespace fin.pointer.contract.impl {

  public partial class ContractFactory : IContractFactory {

    // TODO: Add tests.
    public IWeakContractSet<T> NewWeakSet<T>() => new WeakContractSet<T>();

    private class WeakContractSet<T> : ContractSetImpl<T>, IWeakContractSet<T> {
    }
  }
}