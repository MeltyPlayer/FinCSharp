using fin.emulation.gb.memory.mapper;

namespace fin.emulation.gb.memory.io {
  public sealed class Ly : BWrappedIoAddress {
    public Ly() :
        base(IMemorySourceFactory.INSTANCE.BuildValue()
                                 .OnSet((value, direct) => {
                                   if (!direct) {
                                     return 0;
                                   }
                                   return value;
                                 })
                                 .Build()
        ) {}
  }
}