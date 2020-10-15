using fin.assert;
using fin.emulation.gb.memory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers.emulation.gb.memory {
  public class ReplayDoubleRegister : IDoubleRegister {
    private readonly string name_;
    private readonly ReplaySingleRegister[] registers_;

    public ReplayDoubleRegister(
        string upperName,
        string lowerName,
        ushort defaultValue,
        LockState defaultLockState = LockState.LOCKED) {
      this.name_ = "" + upperName + lowerName;
      var (upperDefaultValue, lowerDefaultValue) =
          ByteMath.SplitShort(defaultValue);
      this.Upper_R =
          new ReplaySingleRegister(upperName,
                                   upperDefaultValue,
                                   defaultLockState);
      this.Lower_R =
          new ReplaySingleRegister(lowerName,
                                   lowerDefaultValue,
                                   defaultLockState);

      this.registers_ = new[] {this.Upper_R, this.Lower_R};
    }

    public void Reset() {
      foreach (var register in this.registers_) {
        register.Reset();
      }
    }

    public LockState LockState {
      get {
        var strongestLock = LockState.CAN_READ_AND_WRITE;
        foreach (var register in this.registers_) {
          var bitLockState = register.LockState;
          if (strongestLock < bitLockState) {
            strongestLock = bitLockState;
          }
        }
        return strongestLock;
      }
      set {
        foreach (var register in this.registers_) {
          register.LockState = value;
        }
      }
    }

    public bool CanRead => this.LockState <= LockState.CAN_READ;
    public bool CanWrite => this.LockState == LockState.CAN_READ_AND_WRITE;

    private void AssertCanRead_()
      => Asserts.True(this.CanRead, "Tried to get locked double register \"" + this.name_ + "\"!");

    private void AssertCanWrite_()
      => Asserts.True(this.CanWrite, "Tried to set locked double register \"" + this.name_ + "\"!");

    public ushort Value {
      get {
        this.AssertCanRead_();
        return ByteMath.MergeBytes(this.Upper.Value, this.Lower.Value);
      }
      set {
        this.AssertCanWrite_();
        (this.Upper.Value, this.Lower.Value) = ByteMath.SplitShort(value);
      }
    }

    public ReplaySingleRegister Upper_R { get; }
    public ReplaySingleRegister Lower_R { get; }

    public ISingleRegister Upper => this.Upper_R;
    public ISingleRegister Lower => this.Lower_R;
  }
}