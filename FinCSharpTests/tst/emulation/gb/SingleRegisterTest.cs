using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb {
  [TestClass]
  public class SingleRegisterTest {
    [TestMethod]
    public void TestSetAndGetValue() {
      var register = new SingleRegister();
      register.Value = 234;
      Assert.AreEqual(234, register.Value);
    }


    [TestMethod]
    public void TestShiftLeft() {
      var register = new SingleRegister();
      register.Value = 0x80;

      register.ShiftLeft(1, out var carry);

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(1, carry);
    }

    [TestMethod]
    public void TestShiftLeftBit() {
      var register = new SingleRegister();
      register.Value = 0x01;

      register.ShiftLeft(1, out var carry);

      Assert.AreEqual(0x02, register.Value);
      Assert.AreEqual(0, carry);
    }

    [TestMethod]
    public void TestShiftLeftHex() {
      var register = new SingleRegister();
      register.Value = 0x0a;

      register.ShiftLeft(4, out var carry);

      Assert.AreEqual(0xa0, register.Value);
      Assert.AreEqual(0, carry);
    }

    [TestMethod]
    public void TestShiftLeftByZero() {
      var register = new SingleRegister();
      register.Value = 0xff;

      register.ShiftLeft(0, out var carry);

      Assert.AreEqual(0xff, register.Value);
      Assert.AreEqual(0, carry);
    }

    [TestMethod]
    public void TestShiftLeftHuge() {
      var register = new SingleRegister();
      register.Value = 0xff;

      register.ShiftLeft(255, out var carry);

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(0xff, carry);
    }


    [TestMethod]
    public void TestShiftRight() {
      var register = new SingleRegister();
      register.Value = 0x01;

      register.ShiftRight(1, out var carry);

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(1, carry);
    }

    [TestMethod]
    public void TestShiftRightBit() {
      var register = new SingleRegister();
      register.Value = 0x02;

      register.ShiftRight(1, out var carry);

      Assert.AreEqual(0x01, register.Value);
      Assert.AreEqual(0, carry);
    }

    [TestMethod]
    public void TestShiftRightHex() {
      var register = new SingleRegister();
      register.Value = 0xa0;

      register.ShiftRight(4, out var carry);

      Assert.AreEqual(0x0a, register.Value);
      Assert.AreEqual(0, carry);
    }

    [TestMethod]
    public void TestShiftRightByZero() {
      var register = new SingleRegister();
      register.Value = 0xff;

      register.ShiftRight(0, out var carry);

      Assert.AreEqual(0xff, register.Value);
      Assert.AreEqual(0, carry);
    }

    [TestMethod]
    public void TestShiftRightHuge() {
      var register = new SingleRegister();
      register.Value = 0xff;

      register.ShiftRight(255, out var carry);

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(0xff, carry);
    }


    [TestMethod]
    public void TestRotateLeftHex() {
      var register = new SingleRegister();
      register.Value = 0xab;

      register.RotateLeft(4, out var carry);

      Assert.AreEqual(0xba, register.Value);
      Assert.AreEqual(0xa, carry);
    }

    [TestMethod]
    public void TestRotateRightHex() {
      var register = new SingleRegister();
      register.Value = 0xab;

      register.RotateRight(4, out var carry);

      Assert.AreEqual(0xba, register.Value);
      Assert.AreEqual(0xb, carry);
    }
  }
}