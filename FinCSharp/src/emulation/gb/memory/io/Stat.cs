namespace fin.emulation.gb.memory.io {
  public enum PpuModeType {
    H_BLANK = 0,
    V_BLANK = 1,
    OAM_RAM_SEARCH = 2,
    DATA_TRANSFER = 3,
  }

  public sealed class Stat : BWrappedIoAddress {
    public PpuModeType Mode {
      get => (PpuModeType) (this.Value & 0x3);
      set => this.Value = (byte) ((this.Value & 0xFC) | (int) value);
    }

    public bool OamRamSearchInterruptEnabled {
      get => BitMath.GetBit(this.Value, 5);
      set => this.Value = (byte) BitMath.SetBit(this.Value, 5, value);
    }

    public bool HBlankInterruptEnabled {
      get => BitMath.GetBit(this.Value, 3);
      set => this.Value = (byte)BitMath.SetBit(this.Value, 3, value);
    }

    public bool VBlankInterruptEnabled {
      get => BitMath.GetBit(this.Value, 4);
      set => this.Value = (byte)BitMath.SetBit(this.Value, 4, value);
    }
  }
}