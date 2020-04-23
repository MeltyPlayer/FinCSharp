namespace SimpleGame.fin.emulation.gb {
  class Processor {
    private void init() {
      // 4. LD n,A
      this.op("LD", "A,A", "7F", 4);
      this.op("LD", "B,A", "47", 4);
      this.op("LD", "C,A", "4F", 4);
      this.op("LD", "D,A", "57", 4);
      this.op("LD", "E,A", "5F", 4);
      this.op("LD", "H,A", "67", 4);
      this.op("LD", "L,A", "6F", 4);
      this.op("LD", "(BC),A", "02", 8);
      this.op("LD", "(DE),A", "12", 8);
      this.op("LD", "(HL),A", "77", 8);
      this.op("LD", "(nn),A", "EA", 16);
    }

    private void op(string instruction,
      string parameters,
      string opcode,
      int cycles) { }
  }
}