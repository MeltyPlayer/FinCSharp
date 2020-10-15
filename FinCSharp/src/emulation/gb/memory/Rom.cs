using fin.file;

namespace fin.emulation.gb.memory {
  public class Rom {
    private readonly byte[] data_;

    public Rom(byte[] data) {
      this.data_ = data;
    }

    public byte this[int bankIndex, ushort relativeAddress]
      => this.data_[bankIndex * 0x4000 + relativeAddress];
  }
}