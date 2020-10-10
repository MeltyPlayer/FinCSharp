using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.memory {
  public abstract class BSingleRegisterTestBase {
    protected abstract ISingleRegister NewRegister(byte value);

    [TestMethod]
    public void TestSetAndGetValue() {
      var register = this.NewRegister(234);
      Assert.AreEqual(234, register.Value);
    }


    [TestMethod]
    public void TestSetAndGetBitOne() {
      var register = this.NewRegister(0x00);
      Assert.AreEqual(false, register.GetBit(3));

      register.SetBit(3, true);

      Assert.AreEqual(true, register.GetBit(3));
    }

    [TestMethod]
    public void TestSetAndGetBitZero() {
      var register = this.NewRegister(0xff);
      Assert.AreEqual(true, register.GetBit(3));

      register.SetBit(3, false);

      Assert.AreEqual(false, register.GetBit(3));
    }


    [TestMethod]
    public void TestSwap() {
      var register = this.NewRegister(0xab);
      Assert.AreEqual(0xba, register.Swap());
      Assert.AreEqual(0xba, register.Value);
    }


    [TestMethod]
    public void TestShiftLeftWithCarry() {
      var register = this.NewRegister(0x80);

      Assert.AreEqual(0, register.LogicalShiftLeft(out var carry));

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestShiftLeftWithoutCarry() {
      var register = this.NewRegister(0x01);

      Assert.AreEqual(0x02, register.LogicalShiftLeft(out var carry));

      Assert.AreEqual(0x02, register.Value);
      Assert.AreEqual(false, carry);
    }


    [TestMethod]
    public void TestShiftRightWithCarry() {
      var register = this.NewRegister(0x01);

      Assert.AreEqual(0, register.LogicalShiftRight(out var carry));

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestShiftRightWithoutCarry() {
      var register = this.NewRegister(0x80);

      Assert.AreEqual(0x40, register.LogicalShiftRight(out var carry));

      Assert.AreEqual(0x40, register.Value);
      Assert.AreEqual(false, carry);
    }


    [TestMethod]
    public void TestRotateLeftBoolFalse() {
      var register = this.NewRegister(0);

      Assert.AreEqual(0, register.RotateLeft(out var carry));

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateLeftBoolTrue() {
      var register = this.NewRegister(0x80);

      Assert.AreEqual(0x01, register.RotateLeft(out var carry));

      Assert.AreEqual(0x01, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateRightBoolFalse() {
      var register = this.NewRegister(0);

      Assert.AreEqual(0, register.RotateRight(out var carry));

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateRightBoolTrue() {
      var register = this.NewRegister(0x01);

      Assert.AreEqual(0x80, register.RotateRight(out var carry));

      Assert.AreEqual(0x80, register.Value);
      Assert.AreEqual(true, carry);
    }


    [TestMethod]
    public void TestRotateLeftThroughCarryFalse() {
      var register = this.NewRegister(0xff);

      var carry = false;
      Assert.AreEqual(0xfe, register.RotateLeftThrough(ref carry));

      Assert.AreEqual(0xfe, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateLeftThroughCarryTrue() {
      var register = this.NewRegister(0x7f);

      var carry = true;
      Assert.AreEqual(0xff, register.RotateLeftThrough(ref carry));

      Assert.AreEqual(0xff, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateLeftThroughCarryFalseEmpty() {
      var register = this.NewRegister(0x80);

      var carry = false;
      Assert.AreEqual(0x00, register.RotateLeftThrough(ref carry));

      Assert.AreEqual(0x00, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateLeftThroughCarryTrueEmpty() {
      var register = this.NewRegister(0x00);

      var carry = true;
      Assert.AreEqual(0x01, register.RotateLeftThrough(ref carry));

      Assert.AreEqual(0x01, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateRightThroughCarryFalse() {
      var register = this.NewRegister(0xff);

      var carry = false;
      Assert.AreEqual(0x7f, register.RotateRightThrough(ref carry));

      Assert.AreEqual(0x7f, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateRightThroughCarryTrue() {
      var register = this.NewRegister(0xfe);

      var carry = true;
      Assert.AreEqual(0xff, register.RotateRightThrough(ref carry));

      Assert.AreEqual(0xff, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateRightThroughCarryFalseEmpty() {
      var register = this.NewRegister(0x01);

      var carry = false;
      Assert.AreEqual(0x00, register.RotateRightThrough(ref carry));

      Assert.AreEqual(0x00, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateRightThroughCarryTrueEmpty() {
      var register = this.NewRegister(0x00);

      var carry = true;
      Assert.AreEqual(0x80, register.RotateRightThrough(ref carry));

      Assert.AreEqual(0x80, register.Value);
      Assert.AreEqual(false, carry);
    }
  }
}