using System.Runtime.CompilerServices;

namespace fin.emulation.gb.memory {
  public class SingleRegister : ISingleRegister {
    public byte Value { get; set; }

    
    public bool GetBit(int index) => ByteMath.GetBit(this.Value, index);

    public void SetBit(int index, bool bit)
      => this.Value = ByteMath.SetBit(this.Value, index, bit);

    
    public byte Swap() => this.Value = ByteMath.Swap(this.Value);

    
    public byte ArithmeticShiftRight(out bool carry)
      => this.Value = ByteMath.ArithmeticShiftRight(this.Value, out carry);

    public byte LogicalShiftLeft(out bool carry)
      => this.Value = ByteMath.LogicalShiftLeft(this.Value, out carry);

    public byte LogicalShiftRight(out bool carry)
      => this.Value = ByteMath.LogicalShiftRight(this.Value, out carry);


    public byte RotateLeft(out bool carry)
      => this.Value = ByteMath.RotateLeft(this.Value, out carry);

    public byte RotateRight(out bool carry)
      => this.Value = ByteMath.RotateRight(this.Value, out carry);


    public byte RotateLeftThrough(ref bool carry)
      => this.Value = ByteMath.RotateLeftThroughCarry(this.Value, ref carry);

    public byte RotateRightThrough(ref bool carry)
      => this.Value = ByteMath.RotateRightThroughCarry(this.Value, ref carry);
  }
}