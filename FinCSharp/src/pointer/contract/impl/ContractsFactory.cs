using System.Collections.Generic;

namespace fin.pointer.contract.impl {

  // TODO: Add tests.
  public partial class ContractFactory : IContractFactory {

    // TODO: Doesn't seem to work.
    static ContractFactory() {
      IContractFactory.DELEGATED_INSTANCE.Add(new ContractFactory());
    }

    public ContractFactory() {
    }
  }
}