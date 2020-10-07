﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    public void TestSetAndGetBitOne() {
      var register = new SingleRegister {Value = 0x00};
      Assert.AreEqual(false, register.GetBit(3));

      register.SetBit(3, true);

      Assert.AreEqual(true, register.GetBit(3));
    }

    [TestMethod]
    public void TestSetAndGetBitZero() {
      var register = new SingleRegister {Value = 0xff};
      Assert.AreEqual(true, register.GetBit(3));

      register.SetBit(3, false);

      Assert.AreEqual(false, register.GetBit(3));
    }


    [TestMethod]
    public void TestShiftLeftWithCarry() {
      var register = new SingleRegister {Value = 0x80};

      Assert.AreEqual(0, register.LogicalShiftLeft(out var carry));

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestShiftLeftWithoutCarry() {
      var register = new SingleRegister {Value = 0x01};

      Assert.AreEqual(0x02, register.LogicalShiftLeft(out var carry));

      Assert.AreEqual(0x02, register.Value);
      Assert.AreEqual(false, carry);
    }


    [TestMethod]
    public void TestShiftRightWithCarry() {
      var register = new SingleRegister {Value = 0x01};

      Assert.AreEqual(0, register.LogicalShiftRight(out var carry));

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestShiftRightWithoutCarry() {
      var register = new SingleRegister {Value = 0x80};

      Assert.AreEqual(0x40, register.LogicalShiftRight(out var carry));

      Assert.AreEqual(0x40, register.Value);
      Assert.AreEqual(false, carry);
    }


    [TestMethod]
    public void TestRotateLeftBoolFalse() {
      var register = new SingleRegister {Value = 0};

      Assert.AreEqual(0, register.RotateLeft(out var carry));

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateLeftBoolTrue() {
      var register = new SingleRegister {Value = 0x80};

      Assert.AreEqual(0x01, register.RotateLeft(out var carry));

      Assert.AreEqual(0x01, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateRightBoolFalse() {
      var register = new SingleRegister {Value = 0};

      Assert.AreEqual(0, register.RotateRight(out var carry));

      Assert.AreEqual(0, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateRightBoolTrue() {
      var register = new SingleRegister {Value = 0x01};

      Assert.AreEqual(0x80, register.RotateRight(out var carry));

      Assert.AreEqual(0x80, register.Value);
      Assert.AreEqual(true, carry);
    }


    [TestMethod]
    public void TestRotateLeftThroughCarryFalse() {
      var register = new SingleRegister {Value = 0xff};

      var carry = false;
      Assert.AreEqual(0xfe, register.RotateLeftThroughCarry(ref carry));

      Assert.AreEqual(0xfe, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateLeftThroughCarryTrue() {
      var register = new SingleRegister {Value = 0x7f};

      var carry = true;
      Assert.AreEqual(0xff, register.RotateLeftThroughCarry(ref carry));

      Assert.AreEqual(0xff, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateLeftThroughCarryFalseEmpty() {
      var register = new SingleRegister {Value = 0x80};

      var carry = false;
      Assert.AreEqual(0x00, register.RotateLeftThroughCarry(ref carry));

      Assert.AreEqual(0x00, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateLeftThroughCarryTrueEmpty() {
      var register = new SingleRegister {Value = 0x00};

      var carry = true;
      Assert.AreEqual(0x01, register.RotateLeftThroughCarry(ref carry));

      Assert.AreEqual(0x01, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateRightThroughCarryFalse() {
      var register = new SingleRegister {Value = 0xff};

      var carry = false;
      Assert.AreEqual(0x7f, register.RotateRightThroughCarry(ref carry));

      Assert.AreEqual(0x7f, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateRightThroughCarryTrue() {
      var register = new SingleRegister {Value = 0xfe};

      var carry = true;
      Assert.AreEqual(0xff, register.RotateRightThroughCarry(ref carry));

      Assert.AreEqual(0xff, register.Value);
      Assert.AreEqual(false, carry);
    }

    [TestMethod]
    public void TestRotateRightThroughCarryFalseEmpty() {
      var register = new SingleRegister {Value = 0x01};

      var carry = false;
      Assert.AreEqual(0x00, register.RotateRightThroughCarry(ref carry));

      Assert.AreEqual(0x00, register.Value);
      Assert.AreEqual(true, carry);
    }

    [TestMethod]
    public void TestRotateRightThroughCarryTrueEmpty() {
      var register = new SingleRegister {Value = 0x00};

      var carry = true;
      Assert.AreEqual(0x80, register.RotateRightThroughCarry(ref carry));

      Assert.AreEqual(0x80, register.Value);
      Assert.AreEqual(false, carry);
    }
  }
}