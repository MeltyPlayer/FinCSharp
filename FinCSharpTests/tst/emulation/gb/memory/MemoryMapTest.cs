using System;

using fin.assert;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.memory {
  [TestClass]
  public class MemoryMapTest {
    private ISerialBus SerialBus { get; set; }
    private MemoryMap MemoryMap { get; set; }

    [TestInitialize]
    public void BeforeEach() {
      this.SerialBus = new SerialBus();
      this.MemoryMap = new MemoryMap(new IoAddresses(this.SerialBus));
    }


    [TestMethod]
    public void TestReadFromBank0() {
      var romData = new byte[0x8000];
      romData[0] = 1;
      this.MemoryMap.Rom = new Rom(romData);

      Assert.AreEqual(1, this.MemoryMap[0]);
    }

    [TestMethod]
    public void TestReadFromBank1() {
      var romData = new byte[0x8000];
      romData[0x4000] = 1;
      this.MemoryMap.Rom = new Rom(romData);

      Assert.AreEqual(1, this.MemoryMap[0x4000]);
    }

    [TestMethod]
    public void TestWriteToBank0ThrowsError() {
      Assert.ThrowsException<AssertionException>(() => this.MemoryMap[0] = 0);
    }

    [TestMethod]
    public void TestWriteToBank1ThrowsError() {
      Assert.ThrowsException<AssertionException>(
          () => this.MemoryMap[0x4000] = 0);
    }

    [TestMethod]
    public void TestWriteToSerialBusPushesChange() {
      var output = "";
      
      this.SerialBus.Bytes.Subscribe(b => output += b);
      this.MemoryMap[0xff01] = 23;

      Assert.AreEqual("23", output);
    }
  }
}