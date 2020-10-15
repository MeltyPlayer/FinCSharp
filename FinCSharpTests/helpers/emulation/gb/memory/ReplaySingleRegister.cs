using fin.assert;
using fin.emulation.gb.memory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers.emulation.gb.memory {
  public class ReplaySingleRegister : ISingleRegister {
    private readonly ISingleRegister impl_;
    private readonly string name_;
    private readonly byte defaultValue_;
    private readonly LockState defaultLockState_;

    public ReplaySingleRegister(
        string name,
        byte defaultValue,
        LockState defaultLockState,
        ISingleRegister? impl = null) {
      this.name_ = name;
      this.defaultValue_ = defaultValue;
      this.defaultLockState_ = defaultLockState;
      this.impl_ = impl ?? new SingleRegister();

      this.Reset();
    }

    public void Reset() {
      this.LockState = LockState.CAN_READ_AND_WRITE;
      this.Value = this.defaultValue_;
      this.LockState = this.defaultLockState_;
    }

    public LockState[] BitLockStates { get; } = new LockState[8];

    public LockState LockState {
      get {
        var strongestLock = LockState.CAN_READ_AND_WRITE;
        foreach (var bitLockState in this.BitLockStates) {
          if (strongestLock < bitLockState) {
            strongestLock = bitLockState;
          }
        }
        return strongestLock;
      }
      set {
        for (var i = 0; i < 8; ++i) {
          this.BitLockStates[i] = value;
        }
      }
    }


    public bool CanReadBit(int index)
      => this.BitLockStates[index] <= LockState.CAN_READ;

    public bool CanRead => this.LockState <= LockState.CAN_READ;

    public bool CanWriteBit(int index)
      => this.BitLockStates[index] == LockState.CAN_READ_AND_WRITE;

    public bool CanWrite => this.LockState == LockState.CAN_READ_AND_WRITE;


    private void AssertCanReadBit_(int index)
      => Asserts.True(this.CanReadBit(index),
                      "Tried to get locked bit " + index +
                      " in single register \"" + this.name_ +
                      "\"!");

    private void AssertCanRead_()
      => Asserts.True(this.CanRead,
                      "Tried to get locked single register \"" + this.name_ +
                      "\"!");

    private void AssertCanWriteBit_(int index)
      => Asserts.True(this.CanWriteBit(index),
                      "Tried to set locked bit " + index +
                      " in single register \"" + this.name_ +
                      "\"!");

    private void AssertCanWrite_()
      => Asserts.True(this.CanWrite,
                      "Tried to set locked single register \"" + this.name_ +
                      "\"!");


    public byte Value {
      get {
        this.AssertCanRead_();
        return this.impl_.Value;
      }
      set {
        this.AssertCanWrite_();
        this.impl_.Value = value;
      }
    }

    public bool GetBit(int index) {
      this.AssertCanReadBit_(index);
      return this.impl_.GetBit(index);
    }

    public void SetBit(int index, bool bit) {
      this.AssertCanWriteBit_(index);
      this.impl_.SetBit(index, bit);
    }

    public byte Swap() {
      this.AssertCanWrite_();
      return this.impl_.Swap();
    }

    public byte ArithmeticShiftRight(out bool carry) {
      this.AssertCanWrite_();
      return this.impl_.ArithmeticShiftRight(out carry);
    }

    public byte LogicalShiftLeft(out bool carry) {
      this.AssertCanWrite_();
      return this.impl_.LogicalShiftLeft(out carry);
    }

    public byte LogicalShiftRight(out bool carry) {
      this.AssertCanWrite_();
      return this.impl_.LogicalShiftRight(out carry);
    }

    public byte RotateLeft(out bool carry) {
      this.AssertCanWrite_();
      return this.impl_.RotateLeft(out carry);
    }

    public byte RotateRight(out bool carry) {
      this.AssertCanWrite_();
      return this.impl_.RotateRight(out carry);
    }

    public byte RotateLeftThrough(ref bool carry) {
      this.AssertCanWrite_();
      return this.impl_.RotateLeftThrough(ref carry);
    }

    public byte RotateRightThrough(ref bool carry) {
      this.AssertCanWrite_();
      return this.impl_.RotateRightThrough(ref carry);
    }
  }
}