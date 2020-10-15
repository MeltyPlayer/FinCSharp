using System.Runtime.InteropServices.ComTypes;

using fin.assert;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.memory {
  [TestClass]
  public class RamRegisterTest : BSingleRegisterTestBase {
    private readonly IMemoryMap memoryMap_ = new MemoryMap(new IoAddresses(new SerialBus()));

    private readonly IDoubleRegister address_ = new DoubleRegister
        {Value = 0x8000};

    protected override ISingleRegister NewRegister(byte value)
      => new RamRegister(this.memoryMap_, this.address_) {Value = value};

    [TestMethod]
    public void TestWritesToMemory() {
      Assert.AreEqual(0, this.memoryMap_[0x8000]);

      var register = this.NewRegister(30);

      Assert.AreEqual(30, this.memoryMap_[0x8000]);
    }
  }
}