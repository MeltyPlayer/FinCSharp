namespace fin.emulation.gb.memory {
  public enum HaltState {
    NOT_HALTED,
    HALTED,
    STOPPED,
  }

  public enum InterruptsState {
    OFF,
    ON,
    SCHEDULED_TO_BE_ON
  }

  public class Memory {
    public IMemoryMap MemoryMap { get; }
    public IRegisters Registers { get; }
    public IStack Stack { get; }

    public HaltState HaltState { get; set; }
    public InterruptsState InterruptsState { get; set; }

    public Memory(IMemoryMap memoryMap, IRegisters registers) {
      this.MemoryMap = memoryMap;
      this.Registers = registers;

      // TODO: Initialize IO memory

      this.Stack = new Stack(this.MemoryMap, this.Registers.Sp);

      this.Reset();
    }

    public void Reset() {
      this.HaltState = HaltState.NOT_HALTED;
      this.InterruptsState = InterruptsState.OFF;

      this.MemoryMap.Reset();
      this.Registers.Reset();
    }
  }
}