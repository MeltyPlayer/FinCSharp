using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.memory {
  [TestClass]
  public class SingleRegisterTest : BSingleRegisterTestBase {
    protected override ISingleRegister NewRegister(byte value)
      => new SingleRegister {Value = value};
  }
}