using fin.assert;
using fin.helpers.emulation.gb;
using fin.helpers.emulation.gb.memory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace fin.emulation.gb {
  [TestClass]
  public class BitOpcodesTest : BOpcodesTestBase {
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
    public void TestBit() {
      var registers = this.ReplayRegisters;
      var bitParamsList = new[] {
          new Params(0x47, 8, () => registers.A_R),
          new Params(0x40, 8, () => registers.B_R),
          new Params(0x41, 8, () => registers.C_R),
          new Params(0x42, 8, () => registers.D_R),
          new Params(0x43, 8, () => registers.E_R),
          new Params(0x44, 8, () => registers.H_R),
          new Params(0x45, 8, () => registers.L_R),
          new Params(0x46, 16, () => this._Hl_),
      };

      foreach (var bitParams in bitParamsList) {
        this.TestBit_SingleRegister(bitParams);
      }
    }

    public void TestBit_SingleRegister(Params bitParams) {
      for (var i = 0; i < 8; ++i) {
        var iParams = new Params((byte) (bitParams.Opcode + i * 0x08),
                                 bitParams.Cycles,
                                 bitParams.GetRegister);
        this.TestBit_SingleIndexZero(iParams, i);
        this.TestBit_SingleIndexOne(iParams, i);
      }
    }

    public void TestBit_SingleIndexZero(Params bitParams, int index) {
      this.Reset();

      var register = bitParams.GetRegister();
      register.BitLockStates[index] = LockState.CAN_READ_AND_WRITE;
      register.SetBit(index, false);
      register.BitLockStates[index] = LockState.CAN_READ;

      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE,
                                  null);
      registers.SetFlagValues(false, true, false, null);

      var actualCycles = this.ExecuteInOrder(1, 0xcb, bitParams.Opcode);

      Assert.AreEqual(4 + bitParams.Cycles, actualCycles);
      registers.AssertFlagValues(true, false, true, null);
    }

    public void TestBit_SingleIndexOne(Params bitParams, int index) {
      this.Reset();

      var register = bitParams.GetRegister();
      register.BitLockStates[index] = LockState.CAN_READ_AND_WRITE;
      register.SetBit(index, true);
      register.BitLockStates[index] = LockState.CAN_READ;

      var registers = this.ReplayRegisters;
      registers.SetFlagLockStates(LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE,
                                  LockState.CAN_READ_AND_WRITE,
                                  null);
      registers.SetFlagValues(true, true, false, null);

      var actualCycles = this.ExecuteInOrder(1, 0xcb, bitParams.Opcode);

      Assert.AreEqual(4 + bitParams.Cycles, actualCycles);
      registers.AssertFlagValues(false, false, true, null);
    }

    [TestMethod]
    public void TestSet() {
      var registers = this.ReplayRegisters;
      var setParamsList = new[] {
          new Params(0xc7, 8, () => registers.A_R),
          new Params(0xc0, 8, () => registers.B_R),
          new Params(0xc1, 8, () => registers.C_R),
          new Params(0xc2, 8, () => registers.D_R),
          new Params(0xc3, 8, () => registers.E_R),
          new Params(0xc4, 8, () => registers.H_R),
          new Params(0xc5, 8, () => registers.L_R),
          new Params(0xc6, 16, () => this._Hl_),
      };

      foreach (var setParams in setParamsList) {
        this.TestSet_SingleRegister(setParams);
      }
    }

    public void TestSet_SingleRegister(Params setParams) {
      for (var i = 0; i < 8; ++i) {
        var iParams = new Params((byte) (setParams.Opcode + i * 0x08),
                                 setParams.Cycles,
                                 setParams.GetRegister);
        this.TestSet_SingleIndex(iParams, i);
      }
    }

    public void TestSet_SingleIndex(Params setParams, int index) {
      this.Reset();

      var register = setParams.GetRegister();
      register.BitLockStates[index] = LockState.CAN_READ_AND_WRITE;
      register.SetBit(index, false);

      var actualCycles = this.ExecuteInOrder(1, 0xcb, setParams.Opcode);

      Assert.AreEqual(4 + setParams.Cycles, actualCycles);
      Assert.AreEqual(true, register.GetBit(index));
    }

    [TestMethod]
    public void TestRes() {
      var registers = this.ReplayRegisters;
      var setParamsList = new[] {
          new Params(0x87, 8, () => registers.A_R),
          new Params(0x80, 8, () => registers.B_R),
          new Params(0x81, 8, () => registers.C_R),
          new Params(0x82, 8, () => registers.D_R),
          new Params(0x83, 8, () => registers.E_R),
          new Params(0x84, 8, () => registers.H_R),
          new Params(0x85, 8, () => registers.L_R),
          new Params(0x86, 16, () => this._Hl_),
      };

      foreach (var setParams in setParamsList) {
        this.TestRes_SingleRegister(setParams);
      }
    }

    public void TestRes_SingleRegister(Params resParams) {
      for (var i = 0; i < 8; ++i) {
        var iParams = new Params((byte) (resParams.Opcode + i * 0x08),
                                 resParams.Cycles,
                                 resParams.GetRegister);
        this.TestRes_SingleIndex(iParams, i);
      }
    }

    public void TestRes_SingleIndex(Params resParams, int index) {
      this.Reset();

      var register = resParams.GetRegister();
      register.BitLockStates[index] = LockState.CAN_READ_AND_WRITE;
      register.SetBit(index, true);

      var actualCycles = this.ExecuteInOrder(1, 0xcb, resParams.Opcode);

      Assert.AreEqual(4 + resParams.Cycles, actualCycles);
      Assert.AreEqual(false, register.GetBit(index));
    }
  }
}