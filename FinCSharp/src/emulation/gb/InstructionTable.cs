using System;
using System.Collections.Generic;
using System.Text;

using fin.assert;
using fin.data.collections.grid;
using fin.data.collections.list;
using fin.emulation.gb.memory;

namespace fin.emulation.gb {
  public interface IInstructionTable {
    void Set(byte opcode, int cycles, Action handler);
    void Set(byte opcode, Func<int> handler);
    int Call(byte opcode);
  }

  public class InstructionTableImpl : IInstructionTable {
    private readonly IList<Func<int>?> impl_ = new List<Func<int>?>();

    public InstructionTableImpl() {
      for (var i = 0; i < 256; ++i) {
        this.impl_.Add(null);
      }
    }

    public void Set(byte opcode, int cycles, Action handler)
      => this.Set(opcode,
                  () => {
                    handler();
                    return cycles;
                  });

    public void Set(byte opcode, Func<int> handler) {
      Asserts.Null(
          this.impl_[opcode],
          $"Expected instruction '{ByteFormatter.ToHex8(opcode)}' to not be defined yet.");

      this.impl_[opcode] = handler;
    }

    public int Call(byte opcode) {
      var handler = this.impl_[opcode];
      if (handler == null) {
        Asserts.Fail(
            $"Expected instruction '{ByteFormatter.ToHex8(opcode)}' to be defined.");
      }

      return handler!();
    }
  }
}