using System;

using fin.assert;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.memory.mbc {
  public enum CartridgeType {
    ROM_ONLY = 0,
    ROM_MBC1 = 1,
    ROM_MBC1_RAM = 2,
    ROM_MBC1_RAM_BATTERY = 3,
    ROM_MBC2 = 5,
  }

  public class MemoryBankController : IMemoryBankController {
    private readonly RomBankSwitcher romBank0_;
    private readonly RomBankSwitcher romBankX_;

    public MemoryBankController(
        RomBankSwitcher romBank0,
        RomBankSwitcher romBankX) {
      this.romBank0_ = romBank0;
      this.romBankX_ = romBankX;
    }

    public Rom Rom {
      set {
        this.romBank0_.Rom = this.romBankX_.Rom = value;
        this.romBank0_.OnSet = this.romBankX_.OnSet = null;

        var rawCartridgeType = value[0x0147];
        var cartridgeType = (CartridgeType) rawCartridgeType;
        switch (cartridgeType) {
          case CartridgeType.ROM_ONLY: return;

          case CartridgeType.ROM_MBC1:
            this.Mbc1_();
            return;

          default:
            Asserts.Fail(
                $"Unsupported cartridge type: {ByteFormatter.ToHex8(rawCartridgeType)}");
            return;
        }
      }
    }

    public enum Mbc1MemoryModeType {
      ROM_16MBIT_RAM_8KBYTE = 0,
      ROM_4MBIT_RAM_32KBYTE = 1,
    }

    // TODO: Move this to a subclass.
    private void Mbc1_() {
      var memoryMode = Mbc1MemoryModeType.ROM_16MBIT_RAM_8KBYTE;

      this.romBank0_.OnSet = (address, value) => {
        // 0x2000-0x3fff sets ROM bank
        if (address >= 0x2000) {
          this.romBankX_.Bank = Math.Max(1, value & 0x1f);
          return true;
        }
        return false;
      };

      this.romBankX_.OnSet = (address, value) => {
        // 0x6000-0x7fff sets memory model
        if (address >= 0x2000) {
          memoryMode = (Mbc1MemoryModeType) (value & 0x1);
          return true;
        }
        return false;
      };
    }
  }
}