using System;
using System.Collections.Generic;
using System.Text;

using fin.assert;
using fin.data.collections.grid;
using fin.data.collections.list;

namespace fin.emulation.gb {
  public interface IInstructionTable {
    void Set(byte opcode, int cycles, Action handler);
    void Set(byte opcode, Func<int> handler);
    void Call(byte opcode);
  }

  public class InstructionTableImpl : IInstructionTable {
    private readonly IList<Func<int>?> impl_ =
        new List<Func<int>?>(256);

    public void Set(byte opcode, int cycles, Action handler)
      => this.Set(opcode,
                  () => {
                    handler();
                    return cycles;
                  });

    public void Set(byte opcode, Func<int> handler) {
      Asserts.Null(
          this.impl_[opcode],
          $"Expected instruction '{opcode}' to not be defined yet.");

      this.impl_[opcode] = handler;
    }

    public void Call(byte opcode) {
      var handler = this.impl_[opcode];
      Asserts.Nonnull(
          handler,
          $"Expected instruction '{opcode}' to be defined.");

      handler!();
    }
  }
}