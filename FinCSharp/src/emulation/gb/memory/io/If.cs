using fin.emulation.gb.memory.mapper;

namespace fin.emulation.gb.memory.io {
  public sealed class If : BWrappedIoAddress {
    public If() :
        base(IMemorySourceFactory.INSTANCE.BuildValue()
                                 .OnSet(value => (byte) (value | 0xe0))
                                 .Build()
        ) {}
  }
}