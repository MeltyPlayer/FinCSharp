using System;

namespace fin.emulation.gb {
  public class SingleRegister {
    private static int UNUSED;

    public byte Value { get; set; }

    public byte ShiftLeft(int n) => this.Value <<= n;

    public byte ShiftLeft(int n, out int carry) {
      var cappedN = Math.Min(n, 8);
      var inverseCappedN = 8 - cappedN;

      carry = this.Value >> inverseCappedN;
      return this.Value <<= cappedN;
    }

    public byte ShiftRight(int n) =>
        this.ShiftRight(n, out SingleRegister.UNUSED);

    public byte ShiftRight(int n, out int carry) {
      var cappedN = Math.Min(n, 8);
      var inverseCappedN = 8 - cappedN;

      carry = (byte) (this.Value << inverseCappedN) >> inverseCappedN;
      return this.Value >>= cappedN;
    }


    public byte RotateLeft(int n)
      => this.RotateLeft(n, out SingleRegister.UNUSED);

    public byte RotateLeft(int n, out int carry) {
      var cappedN = n % 8;
      var inverseCappedN = 8 - cappedN;

      var remaining = this.Value << cappedN;
      carry = this.Value >> inverseCappedN;

      return this.Value = (byte) (remaining | carry);
    }

    public byte RotateRight(int n) =>
        this.RotateRight(n, out SingleRegister.UNUSED);

    public byte RotateRight(int n, out int carry) {
      var cappedN = n % 8;
      var inverseCappedN = 8 - cappedN;

      var remaining = this.Value >> cappedN;
      var carried = this.Value << inverseCappedN;

      carry = (byte) carried >> inverseCappedN;
      return this.Value = (byte) (remaining | carried);
    }
  }
}