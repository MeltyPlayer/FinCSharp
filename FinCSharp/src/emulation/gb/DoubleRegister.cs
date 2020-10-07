using System;
using System.Collections.Generic;
using System.Text;

namespace fin.emulation.gb {
  public class DoubleRegister {
    public ushort Value {
      get => ByteMath.MergeBytes(this.Upper.Value, this.Lower.Value);
      set => (this.Upper.Value, this.Lower.Value) = ByteMath.SplitShort(value);
    }

    public SingleRegister Upper { get; } = new SingleRegister();
    public SingleRegister Lower { get; } = new SingleRegister();


    public ushort ArithmeticShiftRight(out bool carry)
      => this.Value =
             (ushort) ByteMath.ArithmeticShiftRight(this.Value, 16, out carry);


    public ushort LogicalShiftLeft(out bool carry)
      => this.Value =
             (ushort) ByteMath.LogicalShiftLeft(this.Value, 16, out carry);

    public ushort LogicalShiftRight(out bool carry)
      => this.Value =
             (ushort) ByteMath.LogicalShiftRight(this.Value, 16, out carry);


    public ushort RotateLeft(out bool carry)
      => this.Value = (ushort) ByteMath.RotateLeft(this.Value, 16, out carry);

    public ushort RotateRight(out bool carry)
      => this.Value = (ushort) ByteMath.RotateRight(this.Value, 16, out carry);


    public ushort RotateLeftThroughCarry(ref bool carry)
      => this.Value =
             (ushort) ByteMath.RotateLeftThroughCarry(
                 this.Value,
                 16,
                 ref carry);

    public ushort RotateRightThroughCarry(ref bool carry)
      => this.Value =
             (ushort) ByteMath.RotateRightThroughCarry(
                 this.Value,
                 16,
                 ref carry);
  }
}