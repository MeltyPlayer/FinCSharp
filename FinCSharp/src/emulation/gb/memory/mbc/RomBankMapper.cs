using System;

using fin.assert;
using fin.emulation.gb.memory.mapper;

namespace fin.emulation.gb.memory.mbc {
  public class RomBankSwitcher : IMemorySource {
    public byte this[ushort address] {
      get {
        Asserts.Nonnull(this.Rom);
        return this.Rom![this.Bank, address];
      }
      set {
        if (!(this.OnSet?.Invoke(address, value) ?? false)) {
          Asserts.Fail(
              $"Attempted to write to ROM at {ByteFormatter.ToHex16(address)}!");
        }
      }
    }

    public int Size => 0x4000;
    public Rom? Rom { get; set; }
    public int Bank { get; set; }
    public Func<ushort, byte, bool>? OnSet { get; set; }
  }
}