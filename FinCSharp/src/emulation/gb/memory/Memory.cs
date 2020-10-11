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
    public IRam Ram { get; }
    public Registers Registers { get; }
    public IStack Stack { get; }

    public HaltState HaltState { get; set; } = HaltState.NOT_HALTED;
    public InterruptsState InterruptsState { get; set; } = InterruptsState.OFF;

    public Memory(IRam ram, Registers registers) {
      this.Ram = ram;
      this.Registers = registers;

      this.Stack = new Stack(this.Ram, this.Registers.Sp);
    }
  }
}