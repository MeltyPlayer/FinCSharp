using System;
using System.Collections.Generic;
using System.Text;

namespace fin.retro.gameboy {
  public enum InstructionType {
    UNKNOWN,
    ADC,
    ADD,
    BIT,
    CALL,
    CCF,
    CP,
    CPL,
    DEC,
    DAA,
    DI,
    HALT,
    INC,
    JP,
    JR,
    LD,
    LDH,
    NOP,
    OR,
    POP,
    PREFIX,
    PUSH,
    RES,
    RET,
    RETI,
    RL,
    RLA,
    RLC,
    RLCA,
    RR,
    RRA,
    RRC,
    RRCA,
    RST,
    SCF,
    SBC,
    SET,
    SLA,
    SRA,
    SRL,
    STOP,
    SUB,
    SWAP,
    XOR,
  }

  public enum Register {
    UNKNOWN,
    A,
    B,
    C,
    D,
    E,
    F,
    H,
    L,
  }

  public enum RegisterDuo {
    UNKNOWN,
    AF,
    BC,
    DE,
    HL,

    /// <summary>Stack pointer</summary>
    SP,

    /// <summary>Program counter</summary>
    PC,
  }

  public enum DataType {

  }

  public class Instruction {

  }
}