using System;

using fin.emulation.gb.memory;

namespace fin.emulation.gb {
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

  public class Cpu {
    private readonly Registers registers_ = new Registers();

    private readonly IInstructionTable instructionTable_ =
        new InstructionTableImpl();

    private readonly IInstructionTable cbInstructionTable_ =
        new InstructionTableImpl();

    private readonly IRam ram_ = new Ram();
    private readonly IStack stack_;

    public ISingleRegister A => this.registers_.A;
    public ISingleRegister B => this.registers_.B;
    public ISingleRegister C => this.registers_.C;
    public ISingleRegister D => this.registers_.D;
    public ISingleRegister E => this.registers_.E;
    public ISingleRegister H => this.registers_.H;
    public ISingleRegister L => this.registers_.L;
    public ISingleRegister Hl { get; }

    public ushort Pc {
      get => this.registers_.Pc.Value;
      set => this.registers_.Pc.Value = value;
    }

    public byte D8 => this.ram_[this.Pc++];

    public ushort D16 {
      get {
        // LSB comes first!
        var lower = this.D8;
        var upper = this.D8;

        return ByteMath.MergeBytes(upper, lower);
      }
    }

    public sbyte R8 => (sbyte) (object) this.ram_[this.Pc++];

    private HaltState haltState_ = HaltState.NOT_HALTED;
    private InterruptsState interruptsState_ = InterruptsState.OFF;

    public Cpu() {
      this.Hl = new RamRegister(this.ram_, this.registers_.Hl);
      this.stack_ = new Stack(this.ram_, this.registers_.Sp);
    }


    private void Op(byte opcode, int cycles, Action handler)
      => this.instructionTable_.Set(opcode, cycles, handler);

    private void Op(byte opcode, Func<int> handler)
      => this.instructionTable_.Set(opcode, handler);

    private delegate int AriZHandler();

    private delegate int AriZcHandler(ref bool c);

    private void AriZc(byte opcode, int cycles, AriZcHandler handler)
      => this.instructionTable_.Set(opcode,
                                    cycles,
                                    () => {
                                      var carry = this.registers_.CFlag;
                                      var result = handler(ref carry);

                                      this.registers_.ZFlag = result == 0;
                                      this.registers_.CFlag = carry;
                                      this.registers_.NFlag =
                                          this.registers_.HFlag = false;
                                    });

    private void CbAriZ(byte opcode, int cycles, AriZHandler handler)
      => this.cbInstructionTable_.Set(opcode,
                                      cycles,
                                      () => {
                                        var result = handler();

                                        this.registers_.ZFlag = result == 0;
                                        this.registers_.CFlag =
                                            this.registers_.NFlag =
                                                this.registers_.HFlag = false;
                                      });

    private void CbAriZc(byte opcode, int cycles, AriZcHandler handler)
      => this.cbInstructionTable_.Set(opcode,
                                      cycles,
                                      () => {
                                        var carry = this.registers_.CFlag;
                                        var result = handler(ref carry);

                                        this.registers_.ZFlag = result == 0;
                                        this.registers_.CFlag = carry;
                                        this.registers_.NFlag =
                                            this.registers_.HFlag = false;
                                      });

    private void Define_() {
      this.Define331_8BitLoads_();
      this.Define332_16BitLoads_();
      this.Define333_8BitAlu_();
      this.Define334_16BitArithmetic_();
      this.Define335_Miscellaneous_();
      this.Define336_RotatesAndShifts_();
      this.Define338_Jumps_();
      this.Define339_Calls_();
      this.Define3310_Restarts_();
      this.Define3311_Returns_();
    }

    private void Define331_8BitLoads_() {}

    private void Define332_16BitLoads_() {}

    private void Define333_8BitAlu_() {}

    private void Define334_16BitArithmetic_() {}

    private void Define335_Miscellaneous_() {
      // 1. SWAP n
      this.CbAriZ(0x37, 8, () => this.registers_.A.Swap());
      this.CbAriZ(0x30, 8, () => this.registers_.B.Swap());
      this.CbAriZ(0x31, 8, () => this.registers_.C.Swap());
      this.CbAriZ(0x32, 8, () => this.registers_.D.Swap());
      this.CbAriZ(0x33, 8, () => this.registers_.E.Swap());
      this.CbAriZ(0x34, 8, () => this.registers_.H.Swap());
      this.CbAriZ(0x35, 8, () => this.registers_.L.Swap());
      this.CbAriZ(0x36, 16, () => this.Hl.Swap());
      // 2. DAA
      this.Op(0x27,
              4,
              () => {
                var a = this.registers_.A.Value;

                // note: assumes a is a uint8_t and wraps from 0xff to 0
                if (!this.registers_.NFlag) {
                  // after an addition, adjust if (half-)carry occurred or if result is out of bounds
                  if (this.registers_.CFlag || a > 0x99) {
                    a += 0x60;
                    this.registers_.CFlag = true;
                  }
                  if (this.registers_.HFlag || (a & 0x0f) > 0x09) {
                    a += 0x6;
                  }
                }
                else {
                  // after a subtraction, only adjust if (half-)carry occurred
                  if (this.registers_.CFlag) {
                    a -= 0x60;
                  }
                  if (this.registers_.HFlag) {
                    a -= 0x6;
                  }
                }

                // these flags are always updated
                this.registers_.ZFlag = (a == 0); // the usual z flag
                this.registers_.HFlag = false;    // h flag is always cleared
              });
      // 3. CPL
      this.Op(0x2f,
              4,
              () => {
                this.A.Value = (byte) ~this.A.Value;
                this.registers_.NFlag = this.registers_.HFlag = true;
              });
      // 4. CCF
      this.Op(0x3f,
              4,
              () => {
                this.registers_.NFlag = this.registers_.HFlag = false;
                this.registers_.CFlag = !this.registers_.CFlag;
              });
      // 5. SCF
      this.Op(0x37,
              4,
              () => {
                this.registers_.NFlag = this.registers_.HFlag = false;
                this.registers_.CFlag = true;
              });
      // 6. NOP
      this.Op(0x00, 4, () => {});
      // 7. HALT
      this.Op(0x76, 4, () => { this.haltState_ = HaltState.HALTED; });
      // 8. STOP
      this.Op(0x10, 4, () => { this.haltState_ = HaltState.STOPPED; });
      // 9. DI
      this.Op(0xf3, 4, () => this.interruptsState_ = InterruptsState.OFF);
      // 10. EI
      this.Op(0xfb,
              4,
              () => this.interruptsState_ = InterruptsState.SCHEDULED_TO_BE_ON);
    }

    private void Define336_RotatesAndShifts_() {
      // 1. RLCA
      this.AriZc(0x07, 4, (ref bool c) => this.A.RotateLeft(out c));
      // 2. RLA
      this.AriZc(0x17, 4, (ref bool c) => this.A.RotateLeftThrough(ref c));
      // 3. RRCA
      this.AriZc(0x0f, 4, (ref bool c) => this.A.RotateRight(out c));
      // 4. RRA
      this.AriZc(0x1f, 4, (ref bool c) => this.A.RotateRightThrough(ref c));
      // 5. RLC n
      this.CbAriZc(0x07, 8, (ref bool c) => this.A.RotateLeft(out c));
      this.CbAriZc(0x00, 8, (ref bool c) => this.B.RotateLeft(out c));
      this.CbAriZc(0x01, 8, (ref bool c) => this.C.RotateLeft(out c));
      this.CbAriZc(0x02, 8, (ref bool c) => this.D.RotateLeft(out c));
      this.CbAriZc(0x03, 8, (ref bool c) => this.E.RotateLeft(out c));
      this.CbAriZc(0x04, 8, (ref bool c) => this.H.RotateLeft(out c));
      this.CbAriZc(0x05, 8, (ref bool c) => this.L.RotateLeft(out c));
      this.CbAriZc(0x06, 16, (ref bool c) => this.Hl.RotateLeft(out c));
      // 6. RL n
      this.CbAriZc(0x17, 8, (ref bool c) => this.A.RotateLeftThrough(ref c));
      this.CbAriZc(0x10, 8, (ref bool c) => this.B.RotateLeftThrough(ref c));
      this.CbAriZc(0x11, 8, (ref bool c) => this.C.RotateLeftThrough(ref c));
      this.CbAriZc(0x12, 8, (ref bool c) => this.D.RotateLeftThrough(ref c));
      this.CbAriZc(0x13, 8, (ref bool c) => this.E.RotateLeftThrough(ref c));
      this.CbAriZc(0x14, 8, (ref bool c) => this.H.RotateLeftThrough(ref c));
      this.CbAriZc(0x15, 8, (ref bool c) => this.L.RotateLeftThrough(ref c));
      this.CbAriZc(0x16, 16, (ref bool c) => this.Hl.RotateLeftThrough(ref c));
      // 7. RRC n
      this.CbAriZc(0x0f, 8, (ref bool c) => this.A.RotateRight(out c));
      this.CbAriZc(0x08, 8, (ref bool c) => this.B.RotateRight(out c));
      this.CbAriZc(0x09, 8, (ref bool c) => this.C.RotateRight(out c));
      this.CbAriZc(0x0a, 8, (ref bool c) => this.D.RotateRight(out c));
      this.CbAriZc(0x0b, 8, (ref bool c) => this.E.RotateRight(out c));
      this.CbAriZc(0x0c, 8, (ref bool c) => this.H.RotateRight(out c));
      this.CbAriZc(0x0d, 8, (ref bool c) => this.L.RotateRight(out c));
      this.CbAriZc(0x0e, 16, (ref bool c) => this.Hl.RotateRight(out c));
      // 8. RR n
      this.CbAriZc(0x1f, 8, (ref bool c) => this.A.RotateRightThrough(ref c));
      this.CbAriZc(0x18, 8, (ref bool c) => this.B.RotateRightThrough(ref c));
      this.CbAriZc(0x19, 8, (ref bool c) => this.C.RotateRightThrough(ref c));
      this.CbAriZc(0x1a, 8, (ref bool c) => this.D.RotateRightThrough(ref c));
      this.CbAriZc(0x1b, 8, (ref bool c) => this.E.RotateRightThrough(ref c));
      this.CbAriZc(0x1c, 8, (ref bool c) => this.H.RotateRightThrough(ref c));
      this.CbAriZc(0x1d, 8, (ref bool c) => this.L.RotateRightThrough(ref c));
      this.CbAriZc(0x1e, 16, (ref bool c) => this.Hl.RotateRightThrough(ref c));
      // 9. SLA n
      this.CbAriZc(0x27, 8, (ref bool c) => this.A.LogicalShiftLeft(out c));
      this.CbAriZc(0x20, 8, (ref bool c) => this.B.LogicalShiftLeft(out c));
      this.CbAriZc(0x21, 8, (ref bool c) => this.C.LogicalShiftLeft(out c));
      this.CbAriZc(0x22, 8, (ref bool c) => this.D.LogicalShiftLeft(out c));
      this.CbAriZc(0x23, 8, (ref bool c) => this.E.LogicalShiftLeft(out c));
      this.CbAriZc(0x24, 8, (ref bool c) => this.H.LogicalShiftLeft(out c));
      this.CbAriZc(0x25, 8, (ref bool c) => this.L.LogicalShiftLeft(out c));
      this.CbAriZc(0x26, 16, (ref bool c) => this.Hl.LogicalShiftLeft(out c));
      // 10. SRA n
      this.CbAriZc(0x2f, 8, (ref bool c) => this.A.ArithmeticShiftRight(out c));
      this.CbAriZc(0x28, 8, (ref bool c) => this.B.ArithmeticShiftRight(out c));
      this.CbAriZc(0x29, 8, (ref bool c) => this.C.ArithmeticShiftRight(out c));
      this.CbAriZc(0x2a, 8, (ref bool c) => this.D.ArithmeticShiftRight(out c));
      this.CbAriZc(0x2b, 8, (ref bool c) => this.E.ArithmeticShiftRight(out c));
      this.CbAriZc(0x2c, 8, (ref bool c) => this.H.ArithmeticShiftRight(out c));
      this.CbAriZc(0x2d, 8, (ref bool c) => this.L.ArithmeticShiftRight(out c));
      this.CbAriZc(0x2e,
                   16,
                   (ref bool c) => this.Hl.ArithmeticShiftRight(out c));
      // 11. SRL n
      this.CbAriZc(0x3f, 8, (ref bool c) => this.A.LogicalShiftRight(out c));
      this.CbAriZc(0x38, 8, (ref bool c) => this.B.LogicalShiftRight(out c));
      this.CbAriZc(0x39, 8, (ref bool c) => this.C.LogicalShiftRight(out c));
      this.CbAriZc(0x3a, 8, (ref bool c) => this.D.LogicalShiftRight(out c));
      this.CbAriZc(0x3b, 8, (ref bool c) => this.E.LogicalShiftRight(out c));
      this.CbAriZc(0x3c, 8, (ref bool c) => this.H.LogicalShiftRight(out c));
      this.CbAriZc(0x3d, 8, (ref bool c) => this.L.LogicalShiftRight(out c));
      this.CbAriZc(0x3e, 16, (ref bool c) => this.Hl.LogicalShiftRight(out c));
    }

    private void Define337_BitOpcodes_() {}

    private void Define338_Jumps_() {
      // 1. JP nn
      this.Op(0xc3, 12, () => this.Pc = this.D16);
      // 2. JP cc,nn
      this.Op(0xc2,
              () => {
                if (!this.registers_.ZFlag) {
                  this.Pc = this.D16;
                  return 16;
                }
                return 12;
              });
      this.Op(0xca,
              () => {
                if (this.registers_.ZFlag) {
                  this.Pc = this.D16;
                  return 16;
                }
                return 12;
              });
      this.Op(0xd2,
              () => {
                if (!this.registers_.CFlag) {
                  this.Pc = this.D16;
                  return 16;
                }
                return 12;
              });
      this.Op(0xda,
              () => {
                if (this.registers_.CFlag) {
                  this.Pc = this.D16;
                  return 16;
                }
                return 12;
              });
      // 3. JP (HL)
      this.Op(0xe9, 4, () => this.Pc = this.Hl.Value);
      // 4. JR n
      this.Op(0x18, 8, () => this.Pc = (ushort) (this.Pc + this.R8));
      // 5. JR cc,n
      this.Op(0x20,
              () => {
                if (!this.registers_.ZFlag) {
                  this.Pc = (ushort) (this.Pc + this.R8);
                  return 12;
                }
                return 8;
              });
      this.Op(0x28,
              () => {
                if (this.registers_.ZFlag) {
                  this.Pc = (ushort) (this.Pc + this.R8);
                  return 12;
                }
                return 8;
              });
      this.Op(0x30,
              () => {
                if (!this.registers_.CFlag) {
                  this.Pc = (ushort) (this.Pc + this.R8);
                  return 12;
                }
                return 8;
              });
      this.Op(0x38,
              () => {
                if (this.registers_.CFlag) {
                  this.Pc = this.D16;
                  return 12;
                }
                return 8;
              });
    }

    private void Call_() {
      this.stack_.Push16((ushort) (this.Pc + 1));
      this.Pc = this.D16;
    }

    private void Define339_Calls_() {
      // 1. CALL nn
      this.Op(0xcd, 12, this.Call_);
      // 1. CALL cc,nn
      this.Op(0xc4,
              () => {
                if (!this.registers_.ZFlag) {
                  this.Call_();
                  return 24;
                }
                return 12;
              });
      this.Op(0xcc,
              () => {
                if (this.registers_.ZFlag) {
                  this.Call_();
                  return 24;
                }
                return 12;
              });
      this.Op(0xd4,
              () => {
                if (!this.registers_.CFlag) {
                  this.Call_();
                  return 24;
                }
                return 12;
              });
      this.Op(0xdc,
              () => {
                if (this.registers_.CFlag) {
                  this.Call_();
                  return 24;
                }
                return 12;
              });
    }

    private void Define3310_Restarts_() {
      // 1. RST n
      this.Op(0xc7,
              16,
              () => {
                this.stack_.Push16(this.Pc);
                this.Pc = 0x00;
              });
      this.Op(0xcf,
              16,
              () => {
                this.stack_.Push16(this.Pc);
                this.Pc = 0x08;
              });
      this.Op(0xd7,
              16,
              () => {
                this.stack_.Push16(this.Pc);
                this.Pc = 0x10;
              });
      this.Op(0xdf,
              16,
              () => {
                this.stack_.Push16(this.Pc);
                this.Pc = 0x18;
              });
      this.Op(0xe7,
              16,
              () => {
                this.stack_.Push16(this.Pc);
                this.Pc = 0x20;
              });
      this.Op(0xef,
              16,
              () => {
                this.stack_.Push16(this.Pc);
                this.Pc = 0x28;
              });
      this.Op(0xf7,
              16,
              () => {
                this.stack_.Push16(this.Pc);
                this.Pc = 0x30;
              });
      this.Op(0xff,
              16,
              () => {
                this.stack_.Push16(this.Pc);
                this.Pc = 0x38;
              });
    }

    private void Define3311_Returns_() {
      // 1. RET
      this.Op(0xc9, 16, () => this.Pc = this.stack_.Pop16());
      // 2. RET cc
      this.Op(0xc0,
              () => {
                if (!this.registers_.ZFlag) {
                  this.Pc = this.stack_.Pop16();
                  return 20;
                }
                return 8;
              });
      this.Op(0xc8,
              () => {
                if (this.registers_.ZFlag) {
                  this.Pc = this.stack_.Pop16();
                  return 20;
                }
                return 8;
              });
      this.Op(0xd0,
              () => {
                if (!this.registers_.CFlag) {
                  this.Pc = this.stack_.Pop16();
                  return 20;
                }
                return 8;
              });
      this.Op(0xd8,
              () => {
                if (this.registers_.CFlag) {
                  this.Pc = this.stack_.Pop16();
                  return 20;
                }
                return 8;
              });
      // 3. RETI
      this.Op(0xd9,
              16,
              () => {
                this.Pc = this.stack_.Pop16();
                this.interruptsState_ = InterruptsState.ON;
              });
    }
  }
}