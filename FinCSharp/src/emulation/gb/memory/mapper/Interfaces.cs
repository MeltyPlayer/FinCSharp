using System;

using fin.emulation.gb.memory.mapper.impl;

namespace fin.emulation.gb.memory.mapper {
  public interface IMemorySource {
    byte this[ushort address] { get; set; }
    int Size { get; }
  }

  
  public interface IMemoryValue : IMemorySource {
    byte Value { get; set; }
  }
  public interface IMemoryValueBuilder {
    /// <summary>
    ///  A handler that should be run on any set.
    /// </summary>
    IMemoryValueBuilder OnSet(Action<byte> handler);
    /// <summary>
    ///  A handler that maps the value during sets.
    /// </summary>
    IMemoryValueBuilder OnSet(Func<byte, byte> mapHandler);

    /// <summary>
    ///  A handler that should be run on any set. Takes in an extra parameter
    ///  for handling direct (through the variable) vs. indirect (via address)
    ///  sets; direct is true and indirect is false.
    /// </summary>
    IMemoryValueBuilder OnSet(Action<byte, bool> handler);
    /// <summary>
    ///  A handler that maps the value during sets. Takes in an extra parameter
    ///  for handling direct (through the variable) vs. indirect (via address)
    ///  sets; direct is true and indirect is false.
    /// </summary>
    IMemoryValueBuilder OnSet(Func<byte, bool, byte> mapHandler);
    
    IMemoryValue Build();
  }


  public interface IMemoryArray : IMemorySource {
    byte[] Values { get; }
  }


  public interface IMemoryMapper : IMemorySource {
    int SourceCount { get; }
  }

  public interface IMemoryMapperBuilder {
    IMemoryMapperBuilder Register(ushort address, IMemorySource source);
    IMemoryMapper Build();
  }

  public interface IMemorySourceFactory {
    public static readonly IMemorySourceFactory INSTANCE = new MemorySourceFactory();

    IMemoryValue NewValue();
    IMemoryValueBuilder BuildValue();

    IMemoryArray NewArray(int size);
    
    IMemoryMapperBuilder BuildMapper(int size);
  }
}