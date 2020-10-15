using System;

using fin.assert;
using fin.emulation.gb.memory;
using fin.helpers.emulation.gb;
using fin.helpers.emulation.gb.memory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb {
  [TestClass]
  public class MiscellaneousOpcodesTest : BOpcodesTestBase {
    public class Params {
      public byte Opcode { get; }
      public int Cycles { get; }
      public Func<ReplaySingleRegister> GetRegister { get; }

      public Params(
          byte opcode,
          int cycles,
          Func<ReplaySingleRegister> getRegister) {
        this.Opcode = opcode;
        this.Cycles = cycles;
        this.GetRegister = getRegister;
      }
    }

    [TestMethod]
    public void TestSwap() {
      var registers = this.ReplayRegisters;
      var swapParamsList = new[] {
          new Params(0x37, 8, () => registers.A_R),
          new Params(0x30, 8, () => registers.B_R),
          new Params(0x31, 8, () => registers.C_R),
          new Params(0x32, 8, () => registers.D_R),
          new Params(0x33, 8, () => registers.E_R),
          new Params(0x34, 8, () => registers.H_R),
          new Params(0x35, 8, () => registers.L_R),
          new Params(0x36, 16, () => this._Hl_),
      };

      foreach (var swapParams in swapParamsList) {
        this.TestSwapSingle(swapParams);
      }
    }

    public void TestSwapSingle(Params swapParams) {
      this.TestSwap_SingleZero(swapParams);
      this.TestSwap_SingleNonZero(swapParams);
    }

    public void TestSwap_SingleZero(Params swapParams) {
      this.Reset();

      var register = swapParams.GetRegister();
      register.LockState = LockState.CAN_READ_AND_WRITE;
      register.Value = 0;

      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(LockState.CAN_READ_AND_WRITE);
      registers.SetFlagValues(false, true, true, true);

      var actualCycles = this.ExecuteInOrder(1, 0xcb, swapParams.Opcode);

      Assert.AreEqual(4 + swapParams.Cycles, actualCycles);
      Assert.AreEqual(0, register.Value);
      registers.AssertFlagValues(true, false, false, false);
    }

    public void TestSwap_SingleNonZero(Params swapParams) {
      this.Reset();

      var register = swapParams.GetRegister();
      register.LockState = LockState.CAN_READ_AND_WRITE;
      register.Value = 0xab;

      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(LockState.CAN_READ_AND_WRITE);
      registers.SetFlagValues(true, true, true, true);

      var actualCycles = this.ExecuteInOrder(1, 0xcb, swapParams.Opcode);

      Assert.AreEqual(4 + swapParams.Cycles, actualCycles);
      Assert.AreEqual(0xba, register.Value);
      registers.AssertFlagValues(false, false, false, false);
    }

    [TestMethod]
    public void TestDaa() {
      Asserts.Fail();
    }

    [TestMethod]
    public void TestCpl() {
      var registers = this.ReplayRegisters;
      var register = registers.A_R;
      register.LockState = LockState.CAN_READ_AND_WRITE;
      register.Value = 0x00;

      registers.SetFlagLockStates(null,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE,
                                  null);
      registers.SetFlagValues(null, false, false, null);

      var actualCycles = this.ExecuteInOrder(1, 0x2f);

      Assert.AreEqual(4, actualCycles);
      Assert.AreEqual(0xff, register.Value);
      registers.AssertFlagValues(null, true, true, null);
    }

    [TestMethod]
    public void TestCcf_OffToOn() {
      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(null,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE);
      registers.SetFlagValues(null, true, true, false);

      var actualCycles = this.ExecuteInOrder(1, 0x3f);

      Assert.AreEqual(4, actualCycles);
      registers.AssertFlagValues(null, false, false, true);
    }

    [TestMethod]
    public void TestCcf_OnToOff() {
      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(null,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE);
      registers.SetFlagValues(null, true, true, true);

      var actualCycles = this.ExecuteInOrder(1, 0x3f);

      Assert.AreEqual(4, actualCycles);
      registers.AssertFlagValues(null, false, false, false);
    }

    [TestMethod]
    public void TestScf() {
      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(null,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE);
      registers.SetFlagValues(null, true, true, false);

      var actualCycles = this.ExecuteInOrder(1, 0x37);

      Assert.AreEqual(4, actualCycles);
      registers.AssertFlagValues(null, false, false, true);
    }

    [TestMethod]
    public void TestNop() {
      var actualCycles = this.ExecuteInOrder(1, 0x00);
      Assert.AreEqual(4, actualCycles);
    }

    [TestMethod]
    public void TestHalt() {
      Assert.AreEqual(HaltState.NOT_HALTED, this.Memory.HaltState);

      var actualCycles = this.ExecuteInOrder(1, 0x76);

      Assert.AreEqual(4, actualCycles);
      Assert.AreEqual(HaltState.HALTED, this.Memory.HaltState);
    }

    // TODO: Test that an interrupt stops the halt.

    [TestMethod]
    public void TestStop() {
      Assert.AreEqual(HaltState.NOT_HALTED, this.Memory.HaltState);

      var actualCycles = this.ExecuteInOrder(1, 0x10);

      Assert.AreEqual(4, actualCycles);
      Assert.AreEqual(HaltState.STOPPED, this.Memory.HaltState);
    }

    // TODO: Test that a button press stops the stop.

    [TestMethod]
    public void TestDi() {
      this.Memory.InterruptsState = InterruptsState.ON;

      var actualCycles = this.ExecuteInOrder(1, 0xf3);

      Assert.AreEqual(4, actualCycles);
      Assert.AreEqual(InterruptsState.OFF, this.Memory.InterruptsState);
    }

    [TestMethod]
    public void TestEi() {
      this.Memory.InterruptsState = InterruptsState.OFF;

      var actualCycles = this.ExecuteInOrder(1, 0xfb);

      Assert.AreEqual(4, actualCycles);
      Assert.AreEqual(InterruptsState.SCHEDULED_TO_BE_ON,
                      this.Memory.InterruptsState);

      this.ExecuteInOrder(1, 0x00);

      Assert.AreEqual(InterruptsState.ON, this.Memory.InterruptsState);
    }
  }
}