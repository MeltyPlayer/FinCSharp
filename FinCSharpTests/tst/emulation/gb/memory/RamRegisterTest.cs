using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.memory {
  [TestClass]
  public class RamRegisterTest : BSingleRegisterTestBase {
    private readonly IRam ram_ = new Ram();
    private readonly IDoubleRegister address_ = new DoubleRegister();

    protected override ISingleRegister NewRegister(byte value)
      => new RamRegister(this.ram_, this.address_) {Value = value};
  }
}