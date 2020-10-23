namespace fin.emulation.gb.memory.io {
  public enum PpuModeType {
    HBLANK = 0,
    VBLANK = 1,
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

    public bool HblankInterruptEnabled {
      get => BitMath.GetBit(this.Value, 3);
      set => this.Value = (byte)BitMath.SetBit(this.Value, 3, value);
    }

    public bool VblankInterruptEnabled {
      get => BitMath.GetBit(this.Value, 4);
      set => this.Value = (byte)BitMath.SetBit(this.Value, 4, value);
    }
  }
}