using fin.assert;
using fin.emulation.gb.memory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers.emulation.gb.memory {
  public enum LockState {
    CAN_READ_AND_WRITE = 0,
    CAN_READ = 1,
    LOCKED = 2,
  }

  public class ReplayRegisters : IRegisters {
    public class ReplayFlag {
      private readonly char name_;
      private readonly ReplaySingleRegister register_;
      private readonly int index_;

      public ReplayFlag(char name, ReplaySingleRegister register, int index) {
        this.name_ = name;
        this.register_ = register;
        this.index_ = index;
      }

      public LockState LockState {
        get => this.register_.BitLockStates[this.index_];
        set => this.register_.BitLockStates[this.index_] = value;
      }

      private string Label_ => $"flag \"{this.name_}\"";

      public bool Value {
        get {
          Asserts.True(this.register_.CanReadBit(this.index_),
                       $"Trying to get locked {this.Label_}!");
          return this.register_.GetBit(this.index_);
        }
        set {
          Asserts.True(this.register_.CanWriteBit(this.index_),
                       $"Trying to set locked {this.Label_}!");
          this.register_.SetBit(this.index_, value);
        }
      }

      public void AssertValue(bool expectedValue) {
        Asserts.Equal(expectedValue,
                      this.Value,
                      $"Expected {this.Label_} to be {expectedValue}!");
      }
    }

    public ReplayRegisters() {
      this.ZFlag_R = new ReplayFlag('z', this.F_R, 7);
      this.NFlag_R = new ReplayFlag('n', this.F_R, 6);
      this.HFlag_R = new ReplayFlag('h', this.F_R, 5);
      this.CFlag_R = new ReplayFlag('c', this.F_R, 4);
    }

    public void Unlock() {
      this.Af_R.LockState = LockState.CAN_READ_AND_WRITE;
      this.Bc_R.LockState = LockState.CAN_READ_AND_WRITE;
      this.De_R.LockState = LockState.CAN_READ_AND_WRITE;
      this.Hl_R.LockState = LockState.CAN_READ_AND_WRITE;
      this.Sp_R.LockState = LockState.CAN_READ_AND_WRITE;
      this.Pc_R.LockState = LockState.CAN_READ_AND_WRITE;
    }

    public void Reset() {
      this.Af_R.Reset();
      this.Bc_R.Reset();
      this.De_R.Reset();
      this.Hl_R.Reset();
      this.Sp_R.Reset();
      this.Pc_R.Reset();
    }

    public ReplayDoubleRegister Af_R { get; } =
      new ReplayDoubleRegister("A", "F", 0x01b0);

    public ReplaySingleRegister A_R => this.Af_R.Upper_R;
    public ReplaySingleRegister F_R => this.Af_R.Lower_R;
    public ReplayFlag ZFlag_R { get; }
    public ReplayFlag NFlag_R { get; }
    public ReplayFlag HFlag_R { get; }
    public ReplayFlag CFlag_R { get; }

    public void SetFlagLockStates(LockState lockState)
      => this.SetFlagLockStates(lockState, lockState, lockState, lockState);

    public void SetFlagLockStates(
        LockState? zLockState,
        LockState? nLockState,
        LockState? hLockState,
        LockState? cLockState) {
      if (zLockState != null) {
        this.ZFlag_R.LockState = (LockState) zLockState;
      }
      if (nLockState != null) {
        this.NFlag_R.LockState = (LockState) nLockState;
      }
      if (hLockState != null) {
        this.HFlag_R.LockState = (LockState) hLockState;
      }
      if (cLockState != null) {
        this.CFlag_R.LockState = (LockState) cLockState;
      }
    }

    public void SetFlagValues(bool value)
      => this.SetFlagValues(value, value, value, value);

    public void SetFlagValues(
        bool? zValue,
        bool? nValue,
        bool? hValue,
        bool? cValue) {
      if (zValue != null) {
        this.ZFlag_R.Value = (bool) zValue;
      }
      if (nValue != null) {
        this.NFlag_R.Value = (bool) nValue;
      }
      if (hValue != null) {
        this.HFlag_R.Value = (bool) hValue;
      }
      if (cValue != null) {
        this.CFlag_R.Value = (bool) cValue;
      }
    }

    public void AssertFlagValues(
        bool? zValue,
        bool? nValue,
        bool? hValue,
        bool? cValue) {
      if (zValue != null) {
        this.ZFlag_R.AssertValue((bool) zValue);
      }
      if (nValue != null) {
        this.NFlag_R.AssertValue((bool) nValue);
      }
      if (hValue != null) {
        this.HFlag_R.AssertValue((bool) hValue);
      }
      if (cValue != null) {
        this.CFlag_R.AssertValue((bool) cValue);
      }
    }


    public ReplayDoubleRegister Bc_R { get; } =
      new ReplayDoubleRegister("B", "C", 0x0013);

    public ReplaySingleRegister B_R => this.Bc_R.Upper_R;
    public ReplaySingleRegister C_R => this.Bc_R.Lower_R;

    public ReplayDoubleRegister De_R { get; } =
      new ReplayDoubleRegister("D", "E", 0x00d8);

    public ReplaySingleRegister D_R => this.De_R.Upper_R;
    public ReplaySingleRegister E_R => this.De_R.Lower_R;

    public ReplayDoubleRegister Hl_R { get; } =
      new ReplayDoubleRegister("H", "L", 0x014d);

    public ReplaySingleRegister H_R => this.De_R.Upper_R;
    public ReplaySingleRegister L_R => this.De_R.Lower_R;

    public ReplayDoubleRegister Sp_R { get; } =
      new ReplayDoubleRegister("S", "P", 0xfffe);

    public ReplayDoubleRegister Pc_R { get; } =
      new ReplayDoubleRegister("P", "C", 0x0100, LockState.CAN_READ_AND_WRITE);


    public IDoubleRegister Af => this.Af_R;
    public ISingleRegister A => this.A_R;
    public ISingleRegister F => this.F_R;

    public bool ZFlag {
      get => this.ZFlag_R.Value;
      set => this.ZFlag_R.Value = value;
    }

    public bool NFlag {
      get => this.NFlag_R.Value;
      set => this.NFlag_R.Value = value;
    }

    public bool HFlag {
      get => this.HFlag_R.Value;
      set => this.HFlag_R.Value = value;
    }

    public bool CFlag {
      get => this.CFlag_R.Value;
      set => this.CFlag_R.Value = value;
    }

    public IDoubleRegister Bc => this.Bc_R;
    public ISingleRegister B => this.B_R;
    public ISingleRegister C => this.C_R;
    public IDoubleRegister De => this.De_R;
    public ISingleRegister D => this.D_R;
    public ISingleRegister E => this.E_R;
    public IDoubleRegister Hl => this.Hl_R;
    public ISingleRegister H => this.H_R;
    public ISingleRegister L => this.L_R;
    public IDoubleRegister Sp => this.Sp_R;

    public IDoubleRegister Pc => this.Pc_R;
  }
}