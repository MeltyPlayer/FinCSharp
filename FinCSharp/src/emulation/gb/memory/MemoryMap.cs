using System;

using fin.assert;
using fin.emulation.gb.memory.io;
using fin.emulation.gb.memory.mapper;
using fin.emulation.gb.memory.mbc;

namespace fin.emulation.gb.memory {
  public class MemoryMap : IMemoryMap {
    private readonly IMemoryMapper impl_;

    private readonly IMemoryBankController mbc_;
    private readonly RomBankSwitcher romBank0_ = new RomBankSwitcher {Bank = 0};
    private readonly RomBankSwitcher romBankX_ = new RomBankSwitcher {Bank = 1};

    public IMemoryArray Vram { get; }
    public IMemoryArray Oam { get; }
    public IoAddresses IoAddresses { get; }

    public MemoryMap(IoAddresses ioAddresses) {
      this.IoAddresses = ioAddresses;
      // TODO: Fix this.
      this.IoAddresses.Parent = this;

      this.mbc_ = new MemoryBankController(this.romBank0_, this.romBankX_);

      var f = IMemorySourceFactory.INSTANCE;

      this.Vram = f.NewArray(0x2000);
      var ram = f.NewArray(0xfe00 - 0xe000);
      this.Oam = f.NewArray(0xa0);

      this.impl_ = f.BuildMapper(0xffff + 1)
                    .Register(0x0000, this.romBank0_)
                    .Register(0x4000, this.romBankX_)
                    .Register(0x8000, this.Vram)
                    .Register(0xc000, ram)
                    .Register(0xe000, ram)
                    .Register(0xfe00, this.Oam)
                    .Register(0xff00, this.IoAddresses)
                    .Build();
      Asserts.Equal(10, this.impl_.SourceCount);

      this.Reset();
    }

    public void Reset() {
      for (var i = 0x8000; i < this.impl_.Size; ++i) {
        if (i != 0xff46) {
          this.impl_[(ushort) i] = 0;
        }
      }

      this.romBankX_.Bank = 1;
      this.IoAddresses.Reset();
    }

    public byte this[ushort address] {
      get => this.impl_[address];
      set => this.impl_[address] = value;
    }

    public Rom Rom {
      set => this.mbc_.Rom = value;
    }
  }
}