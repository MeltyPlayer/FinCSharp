using System;

using fin.emulation.gb.memory;
using fin.helpers.emulation.gb;
using fin.helpers.emulation.gb.memory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace fin.emulation.gb {
  [TestClass]
  public class Arithmetic16BitTest : BOpcodesTestBase {
    private class AddHlParams {
      public ushort Value { get; }
      public ushort Delta { get; }
      public bool Z { get; }
      public bool H { get; }
      public bool C { get; }

      public AddHlParams(
          ushort value,
          ushort delta,
          bool z,
          bool h,
          bool c) {
        this.Value = value;
        this.Delta = delta;
        this.Z = z;
        this.H = h;
        this.C = c;
      }
    }

    [TestMethod]
    public void TestAddHl_ToOther() {
      var addHlParamsList = new[] {
          new AddHlParams(0x0ff0, 0x0010, false, true, false),
          new AddHlParams(0xffff, 0x0001, true, false, true),
      };

      foreach (var addHlParams in addHlParamsList) {
        try {
          this.TestAddHl_ToOther_Case_(addHlParams);
        } catch (Exception e) {
          throw new Exception(JsonConvert.SerializeObject(addHlParams), e);
        }
      }
    }

    private void TestAddHl_ToOther_Case_(AddHlParams addHlParams) {
      this.TestAddHl_ToOther_Opcode_(0x09, this.Bc, addHlParams);
      this.TestAddHl_ToOther_Opcode_(0x19, this.De, addHlParams);
      this.TestAddHl_ToOther_Opcode_(0x39, this.Sp, addHlParams);
    }

    private void TestAddHl_ToOther_Opcode_(
        byte opcode,
        ReplayDoubleRegister rhs,
        AddHlParams addHlParams) {
      this.Reset();

      var lhs = this.Hl;
      lhs.LockState = LockState.CAN_READ_AND_WRITE;
      lhs.Value = addHlParams.Value;

      rhs.LockState = LockState.CAN_READ_AND_WRITE;
      rhs.Value = addHlParams.Delta;
      rhs.LockState = LockState.CAN_READ;

      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(LockState.CAN_READ_AND_WRITE);
      registers.SetFlagValues(!addHlParams.Z,
                              true,
                              !addHlParams.H,
                              !addHlParams.C);

      var actualCycles = this.ExecuteInOrder(1, opcode);

      Assert.AreEqual(8, actualCycles);
      Assert.AreEqual((addHlParams.Value + addHlParams.Delta) & 0xffff,
                      lhs.Value);

      registers.AssertFlagValues(addHlParams.Z,
                                 false,
                                 addHlParams.H,
                                 addHlParams.C);
    }


    private class AddSpParams {
      public ushort Value { get; }
      public sbyte Delta { get; }
      public bool H { get; }
      public bool C { get; }

      public AddSpParams(ushort value, sbyte delta, bool h, bool c) {
        this.Value = value;
        this.Delta = delta;
        this.H = h;
        this.C = c;
      }
    }

    [TestMethod]
    public void TestAddSp() {
      var addSpParamsList = new[] {
          new AddSpParams(0x000f, 0x01, true, false),
          new AddSpParams(0x00f0, 0x10, false, true),
          new AddSpParams(0x0ff0, 0x10, false, false),
          new AddSpParams(0x0100, 0, false, true),
          new AddSpParams(0x0101, -1, false, true),
          new AddSpParams(0x0020, -0x10, true, false),
      };

      foreach (var addSpParams in addSpParamsList) {
        try {
          this.TestAddSp_Case_(addSpParams);
        } catch (Exception e) {
          throw new Exception(JsonConvert.SerializeObject(addSpParams), e);
        }
      }
    }

    private void TestAddSp_Case_(AddSpParams addSpParams) {
      this.Reset();

      var register = this.Sp;
      register.LockState = LockState.CAN_READ_AND_WRITE;
      register.Value = addSpParams.Value;

      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(LockState.CAN_READ_AND_WRITE);
      registers.SetFlagValues(true, true, !addSpParams.H, !addSpParams.C);

      var actualCycles =
          this.ExecuteInOrder(1, 0xe8, ByteMath.SByteToByte(addSpParams.Delta));

      Assert.AreEqual(16, actualCycles);
      Assert.AreEqual(addSpParams.Value + addSpParams.Delta, register.Value);

      registers.AssertFlagValues(false, false, addSpParams.H, addSpParams.C);
    }


    [TestMethod]
    public void TestInc_Bc() => this.TestInc_Single_(0x03, this.Bc);

    [TestMethod]
    public void TestInc_De() => this.TestInc_Single_(0x13, this.De);

    [TestMethod]
    public void TestInc_Hl() => this.TestInc_Single_(0x23, this.Hl);

    [TestMethod]
    public void TestInc_Sp() => this.TestInc_Single_(0x33, this.Sp);

    private void TestInc_Single_(byte opcode, ReplayDoubleRegister register) {
      this.Reset();

      register.LockState = LockState.CAN_READ_AND_WRITE;
      register.Value = 255;

      var actualCycles = this.ExecuteInOrder(1, opcode);

      Assert.AreEqual(8, actualCycles);
      Assert.AreEqual(256, register.Value);
    }


    [TestMethod]
    public void TestDec_Bc() => this.TestDec_Single_(0x0b, this.Bc);

    [TestMethod]
    public void TestDec_De() => this.TestDec_Single_(0x1b, this.De);

    [TestMethod]
    public void TestDec_Hl() => this.TestDec_Single_(0x2b, this.Hl);

    [TestMethod]
    public void TestDec_Sp() => this.TestDec_Single_(0x3b, this.Sp);

    private void TestDec_Single_(byte opcode, ReplayDoubleRegister register) {
      this.Reset();

      register.LockState = LockState.CAN_READ_AND_WRITE;
      register.Value = 256;

      var actualCycles = this.ExecuteInOrder(1, opcode);

      Assert.AreEqual(8, actualCycles);
      Assert.AreEqual(255, register.Value);
    }
  }
}