using System;
using System.Collections.Generic;
using System.Text;

namespace fin.emulation.gb {
  public class DoubleRegister {
    public ushort Value {
      get => DoubleRegister.MergeBytes_(this.Upper.Value, this.Lower.Value);
      set => (this.Upper.Value, this.Lower.Value) =
             DoubleRegister.SplitShort_(value);
    }

    public SingleRegister Upper { get; } = new SingleRegister();
    public SingleRegister Lower { get; } = new SingleRegister();

    private static (byte, byte) SplitShort_(ushort value)
      => ((byte) (value >> 8), (byte) (value & 0xff));

    private static ushort MergeBytes_(byte upper, byte lower)
      => (ushort) ((upper << 8) | lower);
  }
}