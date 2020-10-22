using fin.emulation.gb.memory.mapper;

namespace fin.emulation.gb.memory.io {
  public abstract class BWrappedIoAddress : IMemoryValue {
    private readonly IMemoryValue impl_;
    
    public BWrappedIoAddress(IMemoryValue? impl = null) {
      this.impl_ = impl ?? IMemorySourceFactory.INSTANCE.NewValue();
    }

    public byte this[ushort address] {
      get => this.impl_[address];
      set => this.impl_[address] = value;
    }

    public int Size => this.impl_.Size;

    public byte Value {
      get => this.impl_.Value;
      set => this.impl_.Value = value;
    }
  }
}
