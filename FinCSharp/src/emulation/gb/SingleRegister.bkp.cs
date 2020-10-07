/*using System;

namespace fin.emulation.gb {
  public class SingleRegister {
    private static int UNUSED_CARRY;

    public byte Value { get; set; }

    public bool GetBit(int index) => ((this.Value >> index) & 0x1) == 0x1;

    public void SetBit(int index, bool value)
      => this.Value =
             value
                 ? (byte) (this.Value | (0x1 << index))
                 : (byte) ~(~this.Value | (0x1 << index));

    public byte ShiftLeft(int n) => this.Value <<= n;

    public byte ShiftLeft(int n, out int carry) {
      var cappedN = Math.Min(n, 8);
      var inverseCappedN = 8 - cappedN;

      carry = this.Value >> inverseCappedN;
      return this.Value <<= cappedN;
    }

    public byte ShiftRight(int n) =>
        this.ShiftRight(n, out SingleRegister.UNUSED_CARRY);

    public byte ShiftRight(int n, out int carry) {
      var cappedN = Math.Min(n, 8);
      var inverseCappedN = 8 - cappedN;

      carry = (byte) (this.Value << inverseCappedN) >> inverseCappedN;
      return this.Value >>= cappedN;
    }


    public byte ShiftLeft(out bool carry)
      => this.Value = (byte) ByteMath.ShiftLeft(this.Value, 8, out carry);

    public byte ShiftRight(out bool carry)
      => this.Value = (byte) ByteMath.ShiftRight(this.Value, 8, out carry);


    public byte RotateLeft(int n)
      => this.RotateLeft(n, out SingleRegister.UNUSED_CARRY);

    public byte RotateLeft(out bool carryBool) {
      var result = this.RotateLeft(1, out var carryInt);
      carryBool = (carryInt & 0x1) == 0x1;
      return result;
    }

    public byte RotateLeft(int n, out int carry) {
      var cappedN = n % 8;
      var inverseCappedN = 8 - cappedN;

      var remaining = this.Value << cappedN;
      carry = this.Value >> inverseCappedN;

      return this.Value = (byte) (remaining | carry);
    }

    public byte RotateRight(int n) =>
        this.RotateRight(n, out SingleRegister.UNUSED_CARRY);

    public byte RotateRight(out bool carryBool) {
      var result = this.RotateRight(1, out var carryInt);
      carryBool = (carryInt & 0x1) == 0x1;
      return result;
    }

    public byte RotateRight(int n, out int carry) {
      var cappedN = n % 8;
      var inverseCappedN = 8 - cappedN;

      var remaining = this.Value >> cappedN;
      var carried = this.Value << inverseCappedN;

      carry = (byte) carried >> inverseCappedN;
      return this.Value = (byte) (remaining | carried);
    }


    public byte RotateLeftThroughCarry(ref bool carry)
      => this.Value =
             (byte) ByteMath.RotateLeftThroughCarry(this.Value, 8, ref carry);

    public byte RotateRightThroughCarry(ref bool carry)
      => this.Value =
             (byte) ByteMath.RotateRightThroughCarry(this.Value, 8, ref carry);
  }
}*/