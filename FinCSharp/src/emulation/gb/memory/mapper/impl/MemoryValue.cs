using System;

namespace fin.emulation.gb.memory.mapper.impl {
  public sealed partial class MemorySourceFactory : IMemorySourceFactory {
    public IMemoryValueBuilder BuildValue() => new MemoryValueBuilder();

    private sealed class MemoryValueBuilder : IMemoryValueBuilder {
      private readonly MemoryValue impl_ = new MemoryValue();

      public IMemoryValueBuilder OnSet(Action<byte> handler) {
        this.impl_.OnSet = (value , _) => {
          handler(value);
          return value;
        };
        return this;
      }

      public IMemoryValueBuilder OnSet(Func<byte, byte> mapHandler) {
        this.impl_.OnSet = (value , _) => mapHandler(value);
        return this;
      }

      public IMemoryValueBuilder OnSet(Action<byte, bool> handler) {
        this.impl_.OnSet = (value, direct) => {
          handler(value, direct);
          return value;
        };
        return this;
      }

      public IMemoryValueBuilder OnSet(Func<byte, bool, byte> mapHandler) {
        this.impl_.OnSet = mapHandler;
        return this;
      }

      public IMemoryValue Build() => this.impl_;
    }

    public IMemoryValue NewValue() => new MemoryValue();

    private sealed class MemoryValue : IMemoryValue {
      private byte value_ = 0;
      public byte this[ushort address] {
        get => this.value_;
        set => this.value_ = this.OnSet?.Invoke(value, false) ?? value;
      }

      public int Size => 1;
      public byte Value {
        get => this.value_;
        set => this.value_ = this.OnSet?.Invoke(value, true) ?? value;
      }

      public Func<byte, bool, byte>? OnSet { get; set; }
    }
  }
}