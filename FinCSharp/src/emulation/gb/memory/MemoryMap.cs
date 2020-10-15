using fin.assert;

namespace fin.emulation.gb.memory {
  public class MemoryMap : IMemoryMap {
    // TODO: Switch this out for a different approach; e.g. memory mapper.
    private readonly byte[] data_ = new byte[0x8000];

    public IoAddresses IoAddresses { get; }
    public Rom Rom { get; set; }

    public MemoryMap(IoAddresses ioAddresses) {
      this.IoAddresses = ioAddresses;

      this.Reset();
    }

    public void Reset() {
      for (var i = 0; i < this.data_.Length; ++i) {
        this.data_[i] = 0;
      }

      this.IoAddresses.Reset();
    }


    public byte this[ushort address] {
      get => this.GetOrSet_(address);
      set => this.GetOrSet_(address, value);
    }

    private byte GetOrSet_(ushort address, byte? setValue = null) {
      // ROM, Bank 0
      if (address < 0x4000) {
        Asserts.Null(setValue, "Can't write to ROM!");
        return this.Rom[0, address];
      }
      // ROM, Switchable bank
      if (address < 0x8000) {
        Asserts.Null(setValue, "Can't write to ROM!");
        // TODO: Handle switchable ROM banks.
        return this.Rom[1, (ushort) (address - 0x4000)];
      }

      // TODO: Use different arrays.
      if (address >= 0xe000 && address < 0xfe00) {
        address = (ushort)(0xc000 + (address - 0xe000));
      }


      if (address >= 0xff00) {
        var ioAddress = (byte) (address - 0xff00);
        return (setValue != null)
                   ? this.IoAddresses[ioAddress] = (byte) setValue
                   : this.IoAddresses[ioAddress];
      }
      
      var dataAddress = address - 0x8000;
      return (setValue != null) ?
        this.data_[dataAddress] = (byte) setValue:
        this.data_[dataAddress];
    }
  }
}