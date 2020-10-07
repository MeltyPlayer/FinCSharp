using System;
using System.Runtime.InteropServices;

using fin.discardable;
using fin.emulation.gb;

namespace fin.retro.gameboy {
  public class Cpu : IGameBoyCpu {
    private readonly Registers registers_ = new Registers();

    private readonly IInstructionTable instructionTable_ =
        new InstructionTableImpl();

    private readonly IInstructionTable cbInstructionTable_ =
        new InstructionTableImpl();

    public byte[] Data {
      get => this.data_;
      set {
        this.data_ = value;
        this.signedData_ = MemoryMarshal.Cast<byte, sbyte>(value).ToArray();
      }
    }

    private byte[] data_;
    private sbyte[] signedData_;

    public ushort Hl {
      get => this.registers_.Hl.Value;
      set => this.registers_.Hl.Value = value;
    }

    public ushort Pc {
      get => this.registers_.Pc.Value;
      set => this.registers_.Pc.Value = value;
    }

    public byte D8 => this.Data[this.Pc++];

    public ushort D16 {
      get {
        // LSB comes first!
        var lower = this.D8;
        var upper = this.D8;

        return ByteMath.MergeBytes(upper, lower);
      }
    }

    public sbyte R8 => this.signedData_[this.Pc++];


    private void Op(byte opcode, int cycles, Action handler)
      => this.instructionTable_.Set(opcode, cycles, handler);

    private void Op(byte opcode, Func<int> handler)
      => this.instructionTable_.Set(opcode, handler);

    private delegate int AriZcHandler(ref bool carry);

    private void AriZc(byte opcode, int cycles, AriZcHandler handler)
      => this.instructionTable_.Set(opcode,
                                    cycles,
                                    () => {
                                      var carry = this.registers_.CFlag;
                                      var result =
                                          handler(ref carry);

                                      this.registers_.ZFlag = result == 0;
                                      this.registers_.CFlag = carry;
                                      this.registers_.NFlag =
                                          this.registers_.HFlag = false;
                                    });

    private void CbAriZc(byte opcode, int cycles, AriZcHandler handler)
      => this.cbInstructionTable_.Set(opcode,
                                      cycles,
                                      () => {
                                        var carry = this.registers_.CFlag;
                                        var result =
                                            handler(ref carry);

                                        this.registers_.ZFlag = result == 0;
                                        this.registers_.CFlag = carry;
                                        this.registers_.NFlag =
                                            this.registers_.HFlag = false;
                                      });

    private void Define_() {
      this.Define331Load_();
      this.Define336RotateAndShift_();
      this.Define338Jump_();
    }

    private void Define331Load_() {}

    private void Define336RotateAndShift_() {
      // 1. RLCA
      this.AriZc(0x07,
                 4,
                 (ref bool carry) =>
                     this.registers_.A.RotateLeft(out carry));
      // 2. RLA
      this.AriZc(0x17,
                 4,
                 (ref bool carry) =>
                     this.registers_.A
                         .RotateLeftThroughCarry(ref carry));
      // 3. RRCA
      this.AriZc(0x0f,
                 4,
                 (ref bool carry) =>
                     this.registers_.A.RotateRight(out carry));
      // 4. RRA
      this.AriZc(0x1f,
                 4,
                 (ref bool carry) =>
                     this.registers_.A
                         .RotateRightThroughCarry(ref carry));
      // 5. RLC n
      this.CbAriZc(0x07,
                   8,
                   (ref bool carry) =>
                       this.registers_.A.RotateLeft(out carry));
      this.CbAriZc(0x00,
                   8,
                   (ref bool carry) =>
                       this.registers_.B.RotateLeft(out carry));
      this.CbAriZc(0x01,
                   8,
                   (ref bool carry) =>
                       this.registers_.C.RotateLeft(out carry));
      this.CbAriZc(0x02,
                   8,
                   (ref bool carry) =>
                       this.registers_.D.RotateLeft(out carry));
      this.CbAriZc(0x03,
                   8,
                   (ref bool carry) =>
                       this.registers_.E.RotateLeft(out carry));
      this.CbAriZc(0x04,
                   8,
                   (ref bool carry) =>
                       this.registers_.H.RotateLeft(out carry));
      this.CbAriZc(0x05,
                   8,
                   (ref bool carry) =>
                       this.registers_.L.RotateLeft(out carry));
      this.CbAriZc(0x06,
                   16,
                   (ref bool carry) =>
                       this.registers_.Hl.RotateLeft(out carry));
      // 6. RL n
      this.CbAriZc(0x17,
                   8,
                   (ref bool carry) =>
                       this.registers_.A.RotateLeftThroughCarry(ref carry));
      this.CbAriZc(0x10,
                   8,
                   (ref bool carry) =>
                       this.registers_.B.RotateLeftThroughCarry(ref carry));
      this.CbAriZc(0x11,
                   8,
                   (ref bool carry) =>
                       this.registers_.C.RotateLeftThroughCarry(ref carry));
      this.CbAriZc(0x12,
                   8,
                   (ref bool carry) =>
                       this.registers_.D.RotateLeftThroughCarry(ref carry));
      this.CbAriZc(0x13,
                   8,
                   (ref bool carry) =>
                       this.registers_.E.RotateLeftThroughCarry(ref carry));
      this.CbAriZc(0x14,
                   8,
                   (ref bool carry) =>
                       this.registers_.H.RotateLeftThroughCarry(ref carry));
      this.CbAriZc(0x15,
                   8,
                   (ref bool carry) =>
                       this.registers_.L.RotateLeftThroughCarry(ref carry));
      this.CbAriZc(0x16,
                   16,
                   (ref bool carry) =>
                       this.registers_.Hl.RotateLeftThroughCarry(ref carry));
      // 7. RRC n
      this.CbAriZc(0x0f,
                   8,
                   (ref bool carry) =>
                       this.registers_.A.RotateRight(out carry));
      this.CbAriZc(0x08,
                   8,
                   (ref bool carry) =>
                       this.registers_.B.RotateRight(out carry));
      this.CbAriZc(0x09,
                   8,
                   (ref bool carry) =>
                       this.registers_.C.RotateRight(out carry));
      this.CbAriZc(0x0a,
                   8,
                   (ref bool carry) =>
                       this.registers_.D.RotateRight(out carry));
      this.CbAriZc(0x0b,
                   8,
                   (ref bool carry) =>
                       this.registers_.E.RotateRight(out carry));
      this.CbAriZc(0x0c,
                   8,
                   (ref bool carry) =>
                       this.registers_.H.RotateRight(out carry));
      this.CbAriZc(0x0d,
                   8,
                   (ref bool carry) =>
                       this.registers_.L.RotateRight(out carry));
      this.CbAriZc(0x0e,
                   16,
                   (ref bool carry) =>
                       this.registers_.Hl.RotateRight(out carry));
      // 8. RR n
      this.CbAriZc(0x1f,
                   8,
                   (ref bool carry) =>
                       this.registers_.A.RotateRightThroughCarry(ref carry));
      this.CbAriZc(0x18,
                   8,
                   (ref bool carry) =>
                       this.registers_.B.RotateRightThroughCarry(ref carry));
      this.CbAriZc(0x19,
                   8,
                   (ref bool carry) =>
                       this.registers_.C.RotateRightThroughCarry(ref carry));
      this.CbAriZc(0x1a,
                   8,
                   (ref bool carry) =>
                       this.registers_.D.RotateRightThroughCarry(ref carry));
      this.CbAriZc(0x1b,
                   8,
                   (ref bool carry) =>
                       this.registers_.E.RotateRightThroughCarry(ref carry));
      this.CbAriZc(0x1c,
                   8,
                   (ref bool carry) =>
                       this.registers_.H.RotateRightThroughCarry(ref carry));
      this.CbAriZc(0x1d,
                   8,
                   (ref bool carry) =>
                       this.registers_.L.RotateRightThroughCarry(ref carry));
      this.CbAriZc(0x1e,
                   16,
                   (ref bool carry) =>
                       this.registers_.Hl.RotateRightThroughCarry(ref carry));
      // 9. SLA n
      this.CbAriZc(0x27,
                   8,
                   (ref bool carry) =>
                       this.registers_.A.LogicalShiftLeft(out carry));
      this.CbAriZc(0x20,
                   8,
                   (ref bool carry) =>
                       this.registers_.B.LogicalShiftLeft(out carry));
      this.CbAriZc(0x21,
                   8,
                   (ref bool carry) =>
                       this.registers_.C.LogicalShiftLeft(out carry));
      this.CbAriZc(0x22,
                   8,
                   (ref bool carry) =>
                       this.registers_.D.LogicalShiftLeft(out carry));
      this.CbAriZc(0x23,
                   8,
                   (ref bool carry) =>
                       this.registers_.E.LogicalShiftLeft(out carry));
      this.CbAriZc(0x24,
                   8,
                   (ref bool carry) =>
                       this.registers_.H.LogicalShiftLeft(out carry));
      this.CbAriZc(0x25,
                   8,
                   (ref bool carry) =>
                       this.registers_.L.LogicalShiftLeft(out carry));
      this.CbAriZc(0x26,
                   16,
                   (ref bool carry) =>
                       this.registers_.Hl.LogicalShiftLeft(out carry));
      // 10. SRA n
      this.CbAriZc(0x2f,
                   8,
                   (ref bool carry) =>
                       this.registers_.A.ArithmeticShiftRight(out carry));
      this.CbAriZc(0x28,
                   8,
                   (ref bool carry) =>
                       this.registers_.B.ArithmeticShiftRight(out carry));
      this.CbAriZc(0x29,
                   8,
                   (ref bool carry) =>
                       this.registers_.C.ArithmeticShiftRight(out carry));
      this.CbAriZc(0x2a,
                   8,
                   (ref bool carry) =>
                       this.registers_.D.ArithmeticShiftRight(out carry));
      this.CbAriZc(0x2b,
                   8,
                   (ref bool carry) =>
                       this.registers_.E.LogicalShiftRight(out carry));
      this.CbAriZc(0x2c,
                   8,
                   (ref bool carry) =>
                       this.registers_.H.ArithmeticShiftRight(out carry));
      this.CbAriZc(0x2d,
                   8,
                   (ref bool carry) =>
                       this.registers_.L.ArithmeticShiftRight(out carry));
      this.CbAriZc(0x2e,
                   16,
                   (ref bool carry) =>
                       this.registers_.Hl.ArithmeticShiftRight(out carry));
      // 1a. SRA n
      this.CbAriZc(0x3f,
                   8,
                   (ref bool carry) =>
                       this.registers_.A.LogicalShiftRight(out carry));
      this.CbAriZc(0x38,
                   8,
                   (ref bool carry) =>
                       this.registers_.B.LogicalShiftRight(out carry));
      this.CbAriZc(0x39,
                   8,
                   (ref bool carry) =>
                       this.registers_.C.LogicalShiftRight(out carry));
      this.CbAriZc(0x3a,
                   8,
                   (ref bool carry) =>
                       this.registers_.D.LogicalShiftRight(out carry));
      this.CbAriZc(0x3b,
                   8,
                   (ref bool carry) =>
                       this.registers_.E.LogicalShiftRight(out carry));
      this.CbAriZc(0x3c,
                   8,
                   (ref bool carry) =>
                       this.registers_.H.LogicalShiftRight(out carry));
      this.CbAriZc(0x3d,
                   8,
                   (ref bool carry) =>
                       this.registers_.L.LogicalShiftRight(out carry));
      this.CbAriZc(0x3e,
                   16,
                   (ref bool carry) =>
                       this.registers_.Hl.LogicalShiftRight(out carry));
    }

    private void Define338Jump_() {
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
      this.Op(0xe9, 4, () => this.Pc = this.Hl);
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
                  this.Pc = (ushort) (this.Pc + this.R8);
                  return 12;
                }
                return 8;
              });
    }
  }
}