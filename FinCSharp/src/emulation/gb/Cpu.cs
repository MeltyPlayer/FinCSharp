using System;

using fin.emulation.gb.memory;

namespace fin.emulation.gb {
  public class Cpu {
    private readonly Memory memory_ = new Memory(new Ram(), new Registers());
    private readonly Opcodes opcodes_;

    public Cpu() {
      this.opcodes_ = new Opcodes(this.memory_);
    }

    public void Execute(int remainingCycles) {
      for (; ; ) {
        var cyclesThisIteration = 0;

        // TODO: Handle interrupts

        // Enables interrupts if scheduled by EI
        if (this.memory_.InterruptsState ==
            InterruptsState.SCHEDULED_TO_BE_ON) {
          this.memory_.InterruptsState = InterruptsState.ON;
        }

        // Runs instruction if not halted
        if (this.memory_.HaltState != HaltState.HALTED) {
          var cycles = this.opcodes_.FetchAndRunOp();
          cyclesThisIteration += cycles;
        } else {
          cyclesThisIteration += 4;
        }

        
        // TODO: Update timers, LCD w/ usedCycles

        // Checks if we're out of cycles
        remainingCycles -= cyclesThisIteration;
        if (cyclesThisIteration <= 0) {
          return;
        }
      }
    }
  }
}