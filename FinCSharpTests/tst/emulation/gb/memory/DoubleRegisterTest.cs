using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.memory {
  [TestClass]
  public class DoubleRegisterTest {

    [TestMethod]
    public void TestSetAndGetUpper() {
      var register = new DoubleRegister();
      register.Upper.Value = 234;
      Assert.AreEqual(234, register.Upper.Value);
    }

    [TestMethod]
    public void TestSetUpperAndGetValue() {
      var register = new DoubleRegister();
      register.Upper.Value = 1;
      Assert.AreEqual(256, register.Value);
    }


    [TestMethod]
    public void TestSetAndGetLower() {
      var register = new DoubleRegister();
      register.Lower.Value = 234;
      Assert.AreEqual(234, register.Lower.Value);
    }

    [TestMethod]
    public void TestSetLowerAndGetValue() {
      var register = new DoubleRegister();
      register.Lower.Value = 1;
      Assert.AreEqual(1, register.Value);
    }


    [TestMethod]
    public void TestSetAndGetValue() {
      var register = new DoubleRegister {Value = 12345};
      Assert.AreEqual(12345, register.Value);
    }

    [TestMethod]
    public void TestLowerIsValueForSmallNumbers() {
      var register = new DoubleRegister {Value = 234 };

      Assert.AreEqual(0, register.Upper.Value);
      Assert.AreEqual(234, register.Lower.Value);
    }


    [TestMethod]
    public void TestValueCreepsToUpperForBigNumbers() {
      var register = new DoubleRegister { Value = 257 };

      Assert.AreEqual(1, register.Upper.Value);
      Assert.AreEqual(1, register.Lower.Value);
    }
  }
}
