namespace fin.emulation.gb.memory {
  public interface IMemoryComponent {
    void Reset();
  }

  public interface IMemory : IMemoryComponent {
  }

  public interface IMemoryMap : IMemoryComponent {
    IoAddresses IoAddresses { get; }
    Rom Rom { get; set; }
    byte this[ushort address] { get; set; }
  }

  public interface IStack {
    void Push8(byte value);
    void Push16(ushort value);

    byte Pop8();
    ushort Pop16();
  }

  public interface IRegisters : IMemoryComponent {
    public IDoubleRegister Af { get; }
    public ISingleRegister A { get; }
    public ISingleRegister F { get; }

    public bool ZFlag { get; set; }
    public bool NFlag { get; set; }
    public bool HFlag { get; set; }
    public bool CFlag { get; set; }

    public IDoubleRegister Bc { get; }
    public ISingleRegister B { get; }
    public ISingleRegister C { get; }


    public IDoubleRegister De { get; }
    public ISingleRegister D { get; }
    public ISingleRegister E { get; }


    IDoubleRegister Hl { get; }
    ISingleRegister H { get; }
    ISingleRegister L { get; }

    IDoubleRegister Sp { get; }
    
    IDoubleRegister Pc { get; }
  }

  public interface ISingleRegister {
    public byte Value { get; set; }

    bool GetBit(int index);
    void SetBit(int index, bool bit);

    byte Swap();

    byte ArithmeticShiftRight(out bool carry);
    byte LogicalShiftLeft(out bool carry);
    byte LogicalShiftRight(out bool carry);

    byte RotateLeft(out bool carry);
    byte RotateRight(out bool carry);

    byte RotateLeftThrough(ref bool carry);
    byte RotateRightThrough(ref bool carry);
  }

  public interface IDoubleRegister {
    public ushort Value { get; set; }

    public ISingleRegister Upper { get; }
    public ISingleRegister Lower { get; }
  }
}