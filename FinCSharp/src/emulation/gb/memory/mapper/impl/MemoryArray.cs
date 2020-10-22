using System.Collections.Generic;

namespace fin.emulation.gb.memory.mapper.impl {
  public sealed partial class MemorySourceFactory : IMemorySourceFactory {
    public IMemoryArray NewArray(int size) => new MemoryArray(size);

    private sealed class MemoryArray : IMemoryArray {
      public MemoryArray(int size) {
        this.Values = new byte[size];
      }

      public byte this[ushort address] {
        get => this.Values[address];
        set => this.Values[address] = value;
      }

      public int Size => (ushort) this.Values.Length;
      public byte[] Values { get; }
    }
  }
}