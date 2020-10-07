using System;

namespace fin.emulation.gb {
  public class SingleRegister {
    public byte Value { get; set; }

    public bool GetBit(int index) => ((this.Value >> index) & 0x1) == 0x1;

    public void SetBit(int index, bool value)
      => this.Value =
             value
                 ? (byte) (this.Value | (0x1 << index))
                 : (byte) ~(~this.Value | (0x1 << index));

    public byte ArithmeticShiftRight(out bool carry)
      => this.Value =
             (byte)ByteMath.LogicalShiftRight(this.Value, 8, out carry);


    public byte LogicalShiftLeft(out bool carry)
      => this.Value =
             (byte) ByteMath.LogicalShiftLeft(this.Value, 8, out carry);

    public byte LogicalShiftRight(out bool carry)
      => this.Value =
             (byte) ByteMath.LogicalShiftRight(this.Value, 8, out carry);


    public byte RotateLeft(out bool carry)
      => this.Value = (byte) ByteMath.RotateLeft(this.Value, 8, out carry);

    public byte RotateRight(out bool carry)
      => this.Value = (byte) ByteMath.RotateRight(this.Value, 8, out carry);


    public byte RotateLeftThroughCarry(ref bool carry)
      => this.Value =
             (byte) ByteMath.RotateLeftThroughCarry(this.Value, 8, ref carry);

    public byte RotateRightThroughCarry(ref bool carry)
      => this.Value =
             (byte) ByteMath.RotateRightThroughCarry(this.Value, 8, ref carry);
  }
}