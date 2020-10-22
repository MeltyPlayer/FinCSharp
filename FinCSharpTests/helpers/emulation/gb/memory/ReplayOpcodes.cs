using System.Collections.Generic;

using fin.emulation.gb;
using fin.emulation.gb.memory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers.emulation.gb.memory {
  public class ReplayOpcodes : IOpcodes {
    private readonly Mmu memory_;
    private readonly IOpcodes impl_;
    private readonly Queue<byte> expectedOpcodes_ = new Queue<byte>();

    public ReplayOpcodes(Mmu memory) {
      this.memory_ = memory;
      this.impl_ = new Opcodes(memory);
    }

    public void ResetReplay() => this.expectedOpcodes_.Clear();

    public void NextInReplay(byte expectedOpcode)
      => this.expectedOpcodes_.Enqueue(expectedOpcode);


    public int FetchAndRunOp() {
      Assert.AreNotEqual(0, this.expectedOpcodes_.Count);
      this.AssertNext_(0);

      // 0xcb is a special opcode that runs another instruction internally
      if (this.GetNextOpcode_(0) == 0xcb) {
        this.AssertNext_(1);
      }

      return this.impl_.FetchAndRunOp();
    }

    private byte GetNextOpcode_(int offset)
      => this.memory_.MemoryMap[
          (ushort) (this.memory_.Registers.Pc.Value + offset)];

    private void AssertNext_(int offset) {
      var expectedNextOpcode = this.expectedOpcodes_.Dequeue();
      var actualNextOpcode = this.GetNextOpcode_(offset);
      Assert.AreEqual(expectedNextOpcode, actualNextOpcode);
    }
  }
}