using System;

using fin.emulation.gb.memory;

namespace fin.emulation.gb {
  public class Opcodes : IOpcodes {
    private readonly Mmu memory_;

    private readonly IInstructionTable instructionTable_ =
        new InstructionTableImpl();

    private readonly IInstructionTable cbInstructionTable_ =
        new InstructionTableImpl();

    public Opcodes(Mmu memory) {
      this.memory_ = memory;
      this.MemoryMap = this.memory_.MemoryMap;
      this.Registers = this.memory_.Registers;
      this.Stack = this.memory_.Stack;

      this._Bc_ = new RamRegister(this.MemoryMap, this.Registers.Bc);
      this._De_ = new RamRegister(this.MemoryMap, this.Registers.De);
      this._Hl_ = new RamRegister(this.MemoryMap, this.Registers.Hl);

      this.Define_();
    }

    public int FetchAndRunOp() => this.instructionTable_.Call(this.D8);

    private IMemoryMap MemoryMap { get; }
    private IRegisters Registers { get; }
    private IStack Stack { get; }

    private HaltState HaltState {
      get => this.memory_.HaltState;
      set => this.memory_.HaltState = value;
    }

    private InterruptsState InterruptsState {
      get => this.memory_.InterruptsState;
      set => this.memory_.InterruptsState = value;
    }

    private ISingleRegister A => this.Registers.A;
    private ISingleRegister B => this.Registers.B;
    private ISingleRegister C => this.Registers.C;
    private ISingleRegister D => this.Registers.D;
    private ISingleRegister E => this.Registers.E;
    private ISingleRegister H => this.Registers.H;
    private ISingleRegister L => this.Registers.L;

    private IDoubleRegister Af => this.Registers.Af;
    private IDoubleRegister Bc => this.Registers.Bc;
    private IDoubleRegister De => this.Registers.De;
    private IDoubleRegister Hl => this.Registers.Hl;
    private IDoubleRegister Sp => this.Registers.Sp;

    private ISingleRegister _Bc_ { get; }
    private ISingleRegister _De_ { get; }
    private ISingleRegister _Hl_ { get; }

    private ushort Pc {
      get => this.Registers.Pc.Value;
      set => this.Registers.Pc.Value = value;
    }

    private byte D8 => this.MemoryMap[this.Pc++];

    private ushort D16 {
      get {
        // LSB comes first!
        var lower = this.D8;
        var upper = this.D8;

        return ByteMath.MergeBytes(upper, lower);
      }
    }

    private sbyte R8 => ByteMath.ByteToSByte(this.MemoryMap[this.Pc++]);

    private void Op(byte opcode, int cycles, Action handler)
      => this.instructionTable_.Set(opcode, cycles, handler);

    private void Op(byte opcode, Func<int> handler)
      => this.instructionTable_.Set(opcode, handler);

    private void CbOp(byte opcode, int cycles, Action handler)
      => this.cbInstructionTable_.Set(opcode, cycles, handler);

    private void CbOp(byte opcode, Func<int> handler)
      => this.cbInstructionTable_.Set(opcode, handler);

    private delegate int AriZHandler();

    private delegate void AriCHandler(ref bool c);

    private delegate int AriZcHandler(ref bool c);

    private void AriZc(byte opcode, int cycles, AriZcHandler handler)
      => this.Op(opcode,
                 cycles,
                 () => {
                   var carry = this.Registers.CFlag;
                   var result = handler(ref carry);

                   this.Registers.ZFlag = result == 0;
                   this.Registers.CFlag = carry;
                   this.Registers.NFlag =
                       this.Registers.HFlag = false;
                 });

    private void AriC(byte opcode, int cycles, AriCHandler handler)
      => this.Op(opcode,
                 cycles,
                 () => {
                   var carry = this.Registers.CFlag;
                   handler(ref carry);

                   this.Registers.CFlag = carry;
                   this.Registers.ZFlag =
                       this.Registers.NFlag =
                           this.Registers.HFlag = false;
                 });

    private void CbAriZ(byte opcode, int cycles, AriZHandler handler)
      => this.CbOp(opcode,
                   cycles,
                   () => {
                     var result = handler();

                     this.Registers.ZFlag = result == 0;
                     this.Registers.CFlag =
                         this.Registers.NFlag =
                             this.Registers.HFlag = false;
                   });

    private void CbAriZc(byte opcode, int cycles, AriZcHandler handler)
      => this.CbOp(opcode,
                   cycles,
                   () => {
                     var carry = this.Registers.CFlag;
                     var result = handler(ref carry);

                     this.Registers.ZFlag = result == 0;
                     this.Registers.CFlag = carry;
                     this.Registers.NFlag =
                         this.Registers.HFlag = false;
                   });

    private void Define_() {
      this.Define331_8BitLoads_();
      this.Define332_16BitLoads_();
      this.Define333_8BitAlu_();
      this.Define334_16BitArithmetic_();
      this.Define335_Miscellaneous_();
      this.Define336_RotatesAndShifts_();
      this.Define337_BitOpcodes_();
      this.Define338_Jumps_();
      this.Define339_Calls_();
      this.Define3310_Restarts_();
      this.Define3311_Returns_();

      this.Op(0xcb, () => 4 + this.cbInstructionTable_.Call(this.D8));
    }

    private void Define331_8BitLoads_() {
      // 1. LD nn,n
      this.Op(0x06, 8, () => this.B.Value = this.D8);
      this.Op(0x0e, 8, () => this.C.Value = this.D8);
      this.Op(0x16, 8, () => this.D.Value = this.D8);
      this.Op(0x1e, 8, () => this.E.Value = this.D8);
      this.Op(0x26, 8, () => this.H.Value = this.D8);
      this.Op(0x2e, 8, () => this.L.Value = this.D8);
      this.Op(0x36, 12, () => this._Hl_.Value = this.D8);
      // 2. LD r1,r2
      this.Op(0x40, 4, () => {}); // B=B, NOP
      this.Op(0x41, 4, () => this.B.Value = this.C.Value);
      this.Op(0x42, 4, () => this.B.Value = this.D.Value);
      this.Op(0x43, 4, () => this.B.Value = this.E.Value);
      this.Op(0x44, 4, () => this.B.Value = this.H.Value);
      this.Op(0x45, 4, () => this.B.Value = this.L.Value);
      this.Op(0x46, 8, () => this.B.Value = this._Hl_.Value);

      this.Op(0x48, 4, () => this.C.Value = this.B.Value);
      this.Op(0x49, 4, () => {}); // C=C, NOP
      this.Op(0x4a, 4, () => this.C.Value = this.D.Value);
      this.Op(0x4b, 4, () => this.C.Value = this.E.Value);
      this.Op(0x4c, 4, () => this.C.Value = this.H.Value);
      this.Op(0x4d, 4, () => this.C.Value = this.L.Value);
      this.Op(0x4e, 8, () => this.C.Value = this._Hl_.Value);

      this.Op(0x50, 4, () => this.D.Value = this.B.Value);
      this.Op(0x51, 4, () => this.D.Value = this.C.Value);
      this.Op(0x52, 4, () => {}); // D=D, NOP
      this.Op(0x53, 4, () => this.D.Value = this.E.Value);
      this.Op(0x54, 4, () => this.D.Value = this.H.Value);
      this.Op(0x55, 4, () => this.D.Value = this.L.Value);
      this.Op(0x56, 8, () => this.D.Value = this._Hl_.Value);

      this.Op(0x58, 4, () => this.E.Value = this.B.Value);
      this.Op(0x59, 4, () => this.E.Value = this.C.Value);
      this.Op(0x5a, 4, () => this.E.Value = this.D.Value);
      this.Op(0x5b, 4, () => {}); // E=E, NOP
      this.Op(0x5c, 4, () => this.E.Value = this.H.Value);
      this.Op(0x5d, 4, () => this.E.Value = this.L.Value);
      this.Op(0x5e, 8, () => this.E.Value = this._Hl_.Value);

      this.Op(0x60, 4, () => this.H.Value = this.B.Value);
      this.Op(0x61, 4, () => this.H.Value = this.C.Value);
      this.Op(0x62, 4, () => this.H.Value = this.D.Value);
      this.Op(0x63, 4, () => this.H.Value = this.E.Value);
      this.Op(0x64, 4, () => {}); // H=H, NOP
      this.Op(0x65, 4, () => this.H.Value = this.L.Value);
      this.Op(0x66, 8, () => this.H.Value = this._Hl_.Value);

      this.Op(0x68, 4, () => this.L.Value = this.B.Value);
      this.Op(0x69, 4, () => this.L.Value = this.C.Value);
      this.Op(0x6a, 4, () => this.L.Value = this.D.Value);
      this.Op(0x6b, 4, () => this.L.Value = this.E.Value);
      this.Op(0x6c, 4, () => this.L.Value = this.H.Value);
      this.Op(0x6d, 4, () => {}); // L=L, NOP
      this.Op(0x6e, 8, () => this.L.Value = this._Hl_.Value);

      this.Op(0x70, 8, () => this._Hl_.Value = this.B.Value);
      this.Op(0x71, 8, () => this._Hl_.Value = this.C.Value);
      this.Op(0x72, 8, () => this._Hl_.Value = this.D.Value);
      this.Op(0x73, 8, () => this._Hl_.Value = this.E.Value);
      this.Op(0x74, 8, () => this._Hl_.Value = this.H.Value);
      this.Op(0x75, 8, () => this._Hl_.Value = this.L.Value);
      // 3. LD A,n
      this.Op(0x7f, 4, () => {}); // A=A, NOP
      this.Op(0x78, 4, () => this.A.Value = this.B.Value);
      this.Op(0x79, 4, () => this.A.Value = this.C.Value);
      this.Op(0x7a, 4, () => this.A.Value = this.D.Value);
      this.Op(0x7b, 4, () => this.A.Value = this.E.Value);
      this.Op(0x7c, 4, () => this.A.Value = this.H.Value);
      this.Op(0x7d, 4, () => this.A.Value = this.L.Value);
      this.Op(0x0a, 8, () => this.A.Value = this._Bc_.Value);
      this.Op(0x1a, 8, () => this.A.Value = this._De_.Value);
      this.Op(0x7e, 8, () => this.A.Value = this._Hl_.Value);
      this.Op(0xfa, 16, () => this.A.Value = this.MemoryMap[this.D16]);
      this.Op(0x3e, 8, () => this.A.Value = this.D8);
      // 4. LD n,A
      this.Op(0x47, 4, () => this.B.Value = this.A.Value);
      this.Op(0x4f, 4, () => this.C.Value = this.A.Value);
      this.Op(0x57, 4, () => this.D.Value = this.A.Value);
      this.Op(0x5f, 4, () => this.E.Value = this.A.Value);
      this.Op(0x67, 4, () => this.H.Value = this.A.Value);
      this.Op(0x6f, 4, () => this.L.Value = this.A.Value);
      this.Op(0x02, 8, () => this._Bc_.Value = this.A.Value);
      this.Op(0x12, 8, () => this._De_.Value = this.A.Value);
      this.Op(0x77, 8, () => this._Hl_.Value = this.A.Value);
      this.Op(0xea, 16, () => this.MemoryMap[this.D16] = this.A.Value);
      // 5. LD A,(C)
      this.Op(0xf2,
              8,
              () => this.A.Value =
                        this.MemoryMap[(ushort) (0xff00 + this.C.Value)]);
      // 6. LD (C),A
      this.Op(0xe2,
              8,
              () => this.MemoryMap[(ushort) (0xff00 + this.C.Value)] =
                        this.A.Value);
      // 7. LD A,(HLD) -->
      // 8. LD A,(HL-) -->
      // 9. LDD A,(HL)
      this.Op(0x3a,
              8,
              () => {
                this.A.Value = this._Hl_.Value;
                --this.Hl.Value;
              });
      // 10. LD (HLD),A -->
      // 11. LD (HL-),A -->
      // 12. LDD (HL),A
      this.Op(0x32,
              8,
              () => {
                this._Hl_.Value = this.A.Value;
                --this.Hl.Value;
              });
      // 13. LD A,(HLI) -->
      // 14. LD A,(HL+) -->
      // 15. LDI A,(HL)
      this.Op(0x2a,
              8,
              () => {
                this.A.Value = this._Hl_.Value;
                ++this.Hl.Value;
              });
      // 16. LD (HLI),A -->
      // 17. LD (HL+),A -->
      // 18. LDI (HL),A
      this.Op(0x22,
              8,
              () => {
                this._Hl_.Value = this.A.Value;
                ++this.Hl.Value;
              });
      // 19. LDH (n),A
      this.Op(0xe0,
              12,
              () => this.MemoryMap[(ushort) (0xff00 + this.D8)] = this.A.Value);
      // 20. LD (C),A
      this.Op(0xf0,
              12,
              () => this.A.Value = this.MemoryMap[(ushort) (0xff00 + this.D8)]);
    }

    private void Define332_16BitLoads_() {
      // 1. LD n,nn
      this.Op(0x01, 12, () => this.Bc.Value = this.D16);
      this.Op(0x11, 12, () => this.De.Value = this.D16);
      this.Op(0x21, 12, () => this.Hl.Value = this.D16);
      this.Op(0x31, 12, () => this.Sp.Value = this.D16);
      // 2. LD SP,HL
      this.Op(0xf9, 8, () => this.Sp.Value = this.Hl.Value);
      // 3. LD HL,SP+n -->
      // 4. LDHL SP,n
      this.Op(0xf8,
              12,
              () => {
                var start = this.Sp.Value;
                var delta = this.R8;
                var address = start + delta;
                this.Hl.Value = (ushort) address;

                this.UpdateAddTo16SpFlags_((byte) start, (byte) delta);
              });
      // 5. LD (nn),SP
      this.Op(0x08,
              20,
              () => {
                var address = this.D16;
                this.MemoryMap[address] = this.Sp.Lower.Value;
                this.MemoryMap[(ushort) (address + 1)] = this.Sp.Upper.Value;
              });
      // 6. PUSH nn
      this.Op(0xf5, 16, () => this.Stack.Push16(this.Af.Value));
      this.Op(0xc5, 16, () => this.Stack.Push16(this.Bc.Value));
      this.Op(0xd5, 16, () => this.Stack.Push16(this.De.Value));
      this.Op(0xe5, 16, () => this.Stack.Push16(this.Hl.Value));
      // 7. POP nn
      this.Op(0xf1, 12, () => this.Af.Value = this.Stack.Pop16());
      this.Op(0xc1, 12, () => this.Bc.Value = this.Stack.Pop16());
      this.Op(0xd1, 12, () => this.De.Value = this.Stack.Pop16());
      this.Op(0xe1, 12, () => this.Hl.Value = this.Stack.Pop16());
    }

    private void AddToA(byte value) {
      var startA = this.A.Value;

      var result = this.A.Value + value;
      this.A.Value = (byte) result;

      this.UpdateFlagsAfterAdd_(startA, value, result);
    }

    private void AdcToA(byte value) {
      var startA = this.A.Value;
      var carry = this.Registers.CFlag ? 1 : 0;

      var result = this.A.Value + value + carry;
      this.A.Value = (byte) result;

      this.UpdateFlagsAfterAdd_(startA, value, result, carry);
    }

    private void UpdateFlagsAfterAdd_(
        int start,
        int delta,
        int result,
        int carry = 0) {
      this.Registers.ZFlag = (byte) result == 0;
      this.Registers.NFlag = false;
      this.Registers.HFlag = (start & 0xf) + (delta & 0xf) + carry > 0xf;
      this.Registers.CFlag = (result & 0x100) != 0;
    }

    private void SubFromA(byte value) {
      var startA = this.A.Value;

      var result = startA - value;
      this.A.Value = (byte) result;

      this.UpdateFlagsAfterSubtract_(startA, value, result);
    }

    private void SbcFromA(byte value) {
      var startA = this.A.Value;
      var carry = this.Registers.CFlag ? 1 : 0;

      var result = startA - value - carry;
      this.A.Value = (byte) result;

      this.UpdateFlagsAfterSubtract_(startA, value, result, carry);
    }

    private void UpdateFlagsAfterSubtract_(
        int start,
        int delta,
        int result,
        int carry = 0) {
      this.Registers.ZFlag = (byte) result == 0;
      this.Registers.NFlag = true;
      this.Registers.HFlag = (start & 0xf) - (delta & 0xf) - carry < 0;
      this.Registers.CFlag = result < 0;
    }

    private void AndWithA(byte value) {
      var result = this.A.Value & value;
      this.A.Value = (byte) result;

      this.Registers.ZFlag = this.A.Value == 0;
      this.Registers.NFlag = false;
      this.Registers.HFlag = true;
      this.Registers.CFlag = false;
    }

    private void OrWithA(byte value) {
      var result = this.A.Value | value;
      this.A.Value = (byte) result;

      this.Registers.ZFlag = this.A.Value == 0;
      this.Registers.NFlag =
          this.Registers.HFlag = this.Registers.CFlag = false;
    }

    private void XorWithA(byte value) {
      var result = this.A.Value ^ value;
      this.A.Value = (byte) result;

      this.Registers.ZFlag = this.A.Value == 0;
      this.Registers.NFlag =
          this.Registers.HFlag = this.Registers.CFlag = false;
    }

    private void CompareWithA(byte value) {
      var startA = this.A.Value;
      var result = startA - value;
      this.UpdateFlagsAfterSubtract_(startA, value, result);
    }

    private void Inc(ISingleRegister register) {
      var result = register.Value + 1;
      register.Value = (byte) result;

      this.Registers.ZFlag = register.Value == 0;
      this.Registers.NFlag = false;
      this.Registers.HFlag = (result & 0xf) == 0;
    }

    private void Dec(ISingleRegister register) {
      var result = register.Value - 1;
      register.Value = (byte) result;

      this.Registers.ZFlag = register.Value == 0;
      this.Registers.NFlag = true;
      this.Registers.HFlag = (result & 0xf) == 0xf;
    }

    private void Define333_8BitAlu_() {
      // 1. ADD A,n
      this.Op(0x87, 4, () => this.AddToA(this.A.Value));
      this.Op(0x80, 4, () => this.AddToA(this.B.Value));
      this.Op(0x81, 4, () => this.AddToA(this.C.Value));
      this.Op(0x82, 4, () => this.AddToA(this.D.Value));
      this.Op(0x83, 4, () => this.AddToA(this.E.Value));
      this.Op(0x84, 4, () => this.AddToA(this.H.Value));
      this.Op(0x85, 4, () => this.AddToA(this.L.Value));
      this.Op(0x86, 8, () => this.AddToA(this._Hl_.Value));
      this.Op(0xc6, 8, () => this.AddToA(this.D8));
      // 2. ADC A,n
      this.Op(0x8f, 4, () => this.AdcToA(this.A.Value));
      this.Op(0x88, 4, () => this.AdcToA(this.B.Value));
      this.Op(0x89, 4, () => this.AdcToA(this.C.Value));
      this.Op(0x8a, 4, () => this.AdcToA(this.D.Value));
      this.Op(0x8b, 4, () => this.AdcToA(this.E.Value));
      this.Op(0x8c, 4, () => this.AdcToA(this.H.Value));
      this.Op(0x8d, 4, () => this.AdcToA(this.L.Value));
      this.Op(0x8e, 8, () => this.AdcToA(this._Hl_.Value));
      this.Op(0xce, 8, () => this.AdcToA(this.D8));
      // 3. SUB n
      this.Op(0x97, 4, () => this.SubFromA(this.A.Value));
      this.Op(0x90, 4, () => this.SubFromA(this.B.Value));
      this.Op(0x91, 4, () => this.SubFromA(this.C.Value));
      this.Op(0x92, 4, () => this.SubFromA(this.D.Value));
      this.Op(0x93, 4, () => this.SubFromA(this.E.Value));
      this.Op(0x94, 4, () => this.SubFromA(this.H.Value));
      this.Op(0x95, 4, () => this.SubFromA(this.L.Value));
      this.Op(0x96, 8, () => this.SubFromA(this._Hl_.Value));
      this.Op(0xd6, 8, () => this.SubFromA(this.D8));
      // 4. SBC A,n
      this.Op(0x9f, 4, () => this.SbcFromA(this.A.Value));
      this.Op(0x98, 4, () => this.SbcFromA(this.B.Value));
      this.Op(0x99, 4, () => this.SbcFromA(this.C.Value));
      this.Op(0x9a, 4, () => this.SbcFromA(this.D.Value));
      this.Op(0x9b, 4, () => this.SbcFromA(this.E.Value));
      this.Op(0x9c, 4, () => this.SbcFromA(this.H.Value));
      this.Op(0x9d, 4, () => this.SbcFromA(this.L.Value));
      this.Op(0x9e, 8, () => this.SbcFromA(this._Hl_.Value));
      this.Op(0xde, 8, () => this.SbcFromA(this.D8));
      // 5. AND n
      this.Op(0xa7, 4, () => this.AndWithA(this.A.Value));
      this.Op(0xa0, 4, () => this.AndWithA(this.B.Value));
      this.Op(0xa1, 4, () => this.AndWithA(this.C.Value));
      this.Op(0xa2, 4, () => this.AndWithA(this.D.Value));
      this.Op(0xa3, 4, () => this.AndWithA(this.E.Value));
      this.Op(0xa4, 4, () => this.AndWithA(this.H.Value));
      this.Op(0xa5, 4, () => this.AndWithA(this.L.Value));
      this.Op(0xa6, 8, () => this.AndWithA(this._Hl_.Value));
      this.Op(0xe6, 8, () => this.AndWithA(this.D8));
      // 6. OR n
      this.Op(0xb7, 4, () => this.OrWithA(this.A.Value));
      this.Op(0xb0, 4, () => this.OrWithA(this.B.Value));
      this.Op(0xb1, 4, () => this.OrWithA(this.C.Value));
      this.Op(0xb2, 4, () => this.OrWithA(this.D.Value));
      this.Op(0xb3, 4, () => this.OrWithA(this.E.Value));
      this.Op(0xb4, 4, () => this.OrWithA(this.H.Value));
      this.Op(0xb5, 4, () => this.OrWithA(this.L.Value));
      this.Op(0xb6, 8, () => this.OrWithA(this._Hl_.Value));
      this.Op(0xf6, 8, () => this.OrWithA(this.D8));
      // 7. XOR n
      this.Op(0xaf, 4, () => this.XorWithA(this.A.Value));
      this.Op(0xa8, 4, () => this.XorWithA(this.B.Value));
      this.Op(0xa9, 4, () => this.XorWithA(this.C.Value));
      this.Op(0xaa, 4, () => this.XorWithA(this.D.Value));
      this.Op(0xab, 4, () => this.XorWithA(this.E.Value));
      this.Op(0xac, 4, () => this.XorWithA(this.H.Value));
      this.Op(0xad, 4, () => this.XorWithA(this.L.Value));
      this.Op(0xae, 8, () => this.XorWithA(this._Hl_.Value));
      this.Op(0xee, 8, () => this.XorWithA(this.D8));
      // 8. CP n
      this.Op(0xbf, 4, () => this.CompareWithA(this.A.Value));
      this.Op(0xb8, 4, () => this.CompareWithA(this.B.Value));
      this.Op(0xb9, 4, () => this.CompareWithA(this.C.Value));
      this.Op(0xba, 4, () => this.CompareWithA(this.D.Value));
      this.Op(0xbb, 4, () => this.CompareWithA(this.E.Value));
      this.Op(0xbc, 4, () => this.CompareWithA(this.H.Value));
      this.Op(0xbd, 4, () => this.CompareWithA(this.L.Value));
      this.Op(0xbe, 8, () => this.CompareWithA(this._Hl_.Value));
      this.Op(0xfe, 8, () => this.CompareWithA(this.D8));
      // 9. INC n
      this.Op(0x3c, 4, () => this.Inc(this.A));
      this.Op(0x04, 4, () => this.Inc(this.B));
      this.Op(0x0c, 4, () => this.Inc(this.C));
      this.Op(0x14, 4, () => this.Inc(this.D));
      this.Op(0x1c, 4, () => this.Inc(this.E));
      this.Op(0x24, 4, () => this.Inc(this.H));
      this.Op(0x2c, 4, () => this.Inc(this.L));
      this.Op(0x34, 12, () => this.Inc(this._Hl_));
      // 10. DEC n
      this.Op(0x3d, 4, () => this.Dec(this.A));
      this.Op(0x05, 4, () => this.Dec(this.B));
      this.Op(0x0d, 4, () => this.Dec(this.C));
      this.Op(0x15, 4, () => this.Dec(this.D));
      this.Op(0x1d, 4, () => this.Dec(this.E));
      this.Op(0x25, 4, () => this.Dec(this.H));
      this.Op(0x2d, 4, () => this.Dec(this.L));
      this.Op(0x35, 12, () => this.Dec(this._Hl_));
    }

    private void AddTo16(IDoubleRegister register, ushort value) {
      var start = register.Value;

      var result = start + value;
      register.Value = (ushort) result;

      this.Registers.NFlag = false;
      this.Registers.HFlag = (start & 0xfff) + (value & 0xfff) > 0xfff;
      this.Registers.CFlag = result > 0xffff;
    }

    private void AddTo16Sp(IDoubleRegister register, int value) {
      var start = register.Value;

      var result = start + value;
      register.Value = (ushort) result;

      this.UpdateAddTo16SpFlags_((byte) start, (byte) value);
    }

    private void UpdateAddTo16SpFlags_(byte start, byte delta) {
      this.Registers.ZFlag = false;
      this.Registers.NFlag = false;
      this.Registers.HFlag = (start & 0xf) + (delta & 0xf) > 0xf;
      this.Registers.CFlag = (start & 0xff) + (delta & 0xff) > 0xff;
    }

    private void Define334_16BitArithmetic_() {
      // 1. ADD HL,n
      this.Op(0x09, 8, () => this.AddTo16(this.Hl, this.Bc.Value));
      this.Op(0x19, 8, () => this.AddTo16(this.Hl, this.De.Value));
      this.Op(0x29, 8, () => this.AddTo16(this.Hl, this.Hl.Value));
      this.Op(0x39, 8, () => this.AddTo16(this.Hl, this.Sp.Value));
      // 2. ADD SP,n
      this.Op(0xe8, 16, () => this.AddTo16Sp(this.Sp, this.R8));
      // 3. INC nn
      this.Op(0x03, 8, () => ++this.Bc.Value);
      this.Op(0x13, 8, () => ++this.De.Value);
      this.Op(0x23, 8, () => ++this.Hl.Value);
      this.Op(0x33, 8, () => ++this.Sp.Value);
      // 4. DEC nn
      this.Op(0x0b, 8, () => --this.Bc.Value);
      this.Op(0x1b, 8, () => --this.De.Value);
      this.Op(0x2b, 8, () => --this.Hl.Value);
      this.Op(0x3b, 8, () => --this.Sp.Value);
    }

    private void Define335_Miscellaneous_() {
      // 1. SWAP n
      this.CbAriZ(0x37, 8, () => this.A.Swap());
      this.CbAriZ(0x30, 8, () => this.B.Swap());
      this.CbAriZ(0x31, 8, () => this.C.Swap());
      this.CbAriZ(0x32, 8, () => this.D.Swap());
      this.CbAriZ(0x33, 8, () => this.E.Swap());
      this.CbAriZ(0x34, 8, () => this.H.Swap());
      this.CbAriZ(0x35, 8, () => this.L.Swap());
      this.CbAriZ(0x36, 16, () => this._Hl_.Swap());
      // 2. DAA
      this.Op(0x27,
              4,
              () => {
                var a = this.Registers.A.Value;

                var n = this.Registers.NFlag;
                var h = this.Registers.HFlag;
                var c = this.Registers.CFlag;

                var delta = 0;
                if (h || (!n && (a & 0xf) > 9)) {
                  delta = 6;
                }
                if (c || (!n && a > 0x99)) {
                  delta |= 0x60;
                  this.Registers.CFlag = true;
                }

                a = (byte) (a + (n ? -delta : +delta));

                this.Registers.A.Value = a;

                this.Registers.ZFlag = (a == 0);
                this.Registers.HFlag = false;
              });
      // 3. CPL
      this.Op(0x2f,
              4,
              () => {
                this.A.Value = (byte) ~this.A.Value;
                this.Registers.NFlag = this.Registers.HFlag = true;
              });
      // 4. CCF
      this.Op(0x3f,
              4,
              () => {
                this.Registers.NFlag = this.Registers.HFlag = false;
                this.Registers.CFlag = !this.Registers.CFlag;
              });
      // 5. SCF
      this.Op(0x37,
              4,
              () => {
                this.Registers.NFlag = this.Registers.HFlag = false;
                this.Registers.CFlag = true;
              });
      // 6. NOP
      this.Op(0x00, 4, () => {});
      // 7. HALT
      this.Op(0x76, 4, () => { this.HaltState = HaltState.HALTED; });
      // 8. STOP
      this.Op(0x10, 4, () => { this.HaltState = HaltState.STOPPED; });
      // 9. DI
      this.Op(0xf3, 4, () => this.InterruptsState = InterruptsState.OFF);
      // 10. EI
      this.Op(0xfb,
              4,
              () => this.InterruptsState = InterruptsState.SCHEDULED_TO_BE_ON);
    }

    private void Define336_RotatesAndShifts_() {
      // 1. RLCA
      this.AriC(0x07, 4, (ref bool c) => this.A.RotateLeft(out c));
      // 2. RLA
      this.AriC(0x17, 4, (ref bool c) => this.A.RotateLeftThrough(ref c));
      // 3. RRCA
      this.AriC(0x0f, 4, (ref bool c) => this.A.RotateRight(out c));
      // 4. RRA
      this.AriC(0x1f, 4, (ref bool c) => this.A.RotateRightThrough(ref c));
      // 5. RLC n
      this.CbAriZc(0x07, 8, (ref bool c) => this.A.RotateLeft(out c));
      this.CbAriZc(0x00, 8, (ref bool c) => this.B.RotateLeft(out c));
      this.CbAriZc(0x01, 8, (ref bool c) => this.C.RotateLeft(out c));
      this.CbAriZc(0x02, 8, (ref bool c) => this.D.RotateLeft(out c));
      this.CbAriZc(0x03, 8, (ref bool c) => this.E.RotateLeft(out c));
      this.CbAriZc(0x04, 8, (ref bool c) => this.H.RotateLeft(out c));
      this.CbAriZc(0x05, 8, (ref bool c) => this.L.RotateLeft(out c));
      this.CbAriZc(0x06, 16, (ref bool c) => this._Hl_.RotateLeft(out c));
      // 6. RL n
      this.CbAriZc(0x17, 8, (ref bool c) => this.A.RotateLeftThrough(ref c));
      this.CbAriZc(0x10, 8, (ref bool c) => this.B.RotateLeftThrough(ref c));
      this.CbAriZc(0x11, 8, (ref bool c) => this.C.RotateLeftThrough(ref c));
      this.CbAriZc(0x12, 8, (ref bool c) => this.D.RotateLeftThrough(ref c));
      this.CbAriZc(0x13, 8, (ref bool c) => this.E.RotateLeftThrough(ref c));
      this.CbAriZc(0x14, 8, (ref bool c) => this.H.RotateLeftThrough(ref c));
      this.CbAriZc(0x15, 8, (ref bool c) => this.L.RotateLeftThrough(ref c));
      this.CbAriZc(0x16,
                   16,
                   (ref bool c) => this._Hl_.RotateLeftThrough(ref c));
      // 7. RRC n
      this.CbAriZc(0x0f, 8, (ref bool c) => this.A.RotateRight(out c));
      this.CbAriZc(0x08, 8, (ref bool c) => this.B.RotateRight(out c));
      this.CbAriZc(0x09, 8, (ref bool c) => this.C.RotateRight(out c));
      this.CbAriZc(0x0a, 8, (ref bool c) => this.D.RotateRight(out c));
      this.CbAriZc(0x0b, 8, (ref bool c) => this.E.RotateRight(out c));
      this.CbAriZc(0x0c, 8, (ref bool c) => this.H.RotateRight(out c));
      this.CbAriZc(0x0d, 8, (ref bool c) => this.L.RotateRight(out c));
      this.CbAriZc(0x0e, 16, (ref bool c) => this._Hl_.RotateRight(out c));
      // 8. RR n
      this.CbAriZc(0x1f, 8, (ref bool c) => this.A.RotateRightThrough(ref c));
      this.CbAriZc(0x18, 8, (ref bool c) => this.B.RotateRightThrough(ref c));
      this.CbAriZc(0x19, 8, (ref bool c) => this.C.RotateRightThrough(ref c));
      this.CbAriZc(0x1a, 8, (ref bool c) => this.D.RotateRightThrough(ref c));
      this.CbAriZc(0x1b, 8, (ref bool c) => this.E.RotateRightThrough(ref c));
      this.CbAriZc(0x1c, 8, (ref bool c) => this.H.RotateRightThrough(ref c));
      this.CbAriZc(0x1d, 8, (ref bool c) => this.L.RotateRightThrough(ref c));
      this.CbAriZc(0x1e,
                   16,
                   (ref bool c) => this._Hl_.RotateRightThrough(ref c));
      // 9. SLA n
      this.CbAriZc(0x27, 8, (ref bool c) => this.A.LogicalShiftLeft(out c));
      this.CbAriZc(0x20, 8, (ref bool c) => this.B.LogicalShiftLeft(out c));
      this.CbAriZc(0x21, 8, (ref bool c) => this.C.LogicalShiftLeft(out c));
      this.CbAriZc(0x22, 8, (ref bool c) => this.D.LogicalShiftLeft(out c));
      this.CbAriZc(0x23, 8, (ref bool c) => this.E.LogicalShiftLeft(out c));
      this.CbAriZc(0x24, 8, (ref bool c) => this.H.LogicalShiftLeft(out c));
      this.CbAriZc(0x25, 8, (ref bool c) => this.L.LogicalShiftLeft(out c));
      this.CbAriZc(0x26, 16, (ref bool c) => this._Hl_.LogicalShiftLeft(out c));
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
                   (ref bool c) => this._Hl_.ArithmeticShiftRight(out c));
      // 11. SRL n
      this.CbAriZc(0x3f, 8, (ref bool c) => this.A.LogicalShiftRight(out c));
      this.CbAriZc(0x38, 8, (ref bool c) => this.B.LogicalShiftRight(out c));
      this.CbAriZc(0x39, 8, (ref bool c) => this.C.LogicalShiftRight(out c));
      this.CbAriZc(0x3a, 8, (ref bool c) => this.D.LogicalShiftRight(out c));
      this.CbAriZc(0x3b, 8, (ref bool c) => this.E.LogicalShiftRight(out c));
      this.CbAriZc(0x3c, 8, (ref bool c) => this.H.LogicalShiftRight(out c));
      this.CbAriZc(0x3d, 8, (ref bool c) => this.L.LogicalShiftRight(out c));
      this.CbAriZc(0x3e,
                   16,
                   (ref bool c) => this._Hl_.LogicalShiftRight(out c));
    }

    private void TestBit_(ISingleRegister register, int index) {
      var result = register.GetBit(index);
      this.Registers.ZFlag = !result;
      this.Registers.NFlag = false;
      this.Registers.HFlag = true;
    }

    private void SetBit_(ISingleRegister register, int index)
      => register.SetBit(index, true);

    private void ResetBit_(ISingleRegister register, int index)
      => register.SetBit(index, false);

    private void Define337_BitOpcodes_() {
      // 1. BIT b,r
      this.CbOp(0x47, 8, () => this.TestBit_(this.A, 0));
      this.CbOp(0x4f, 8, () => this.TestBit_(this.A, 1));
      this.CbOp(0x57, 8, () => this.TestBit_(this.A, 2));
      this.CbOp(0x5f, 8, () => this.TestBit_(this.A, 3));
      this.CbOp(0x67, 8, () => this.TestBit_(this.A, 4));
      this.CbOp(0x6f, 8, () => this.TestBit_(this.A, 5));
      this.CbOp(0x77, 8, () => this.TestBit_(this.A, 6));
      this.CbOp(0x7f, 8, () => this.TestBit_(this.A, 7));

      this.CbOp(0x40, 8, () => this.TestBit_(this.B, 0));
      this.CbOp(0x48, 8, () => this.TestBit_(this.B, 1));
      this.CbOp(0x50, 8, () => this.TestBit_(this.B, 2));
      this.CbOp(0x58, 8, () => this.TestBit_(this.B, 3));
      this.CbOp(0x60, 8, () => this.TestBit_(this.B, 4));
      this.CbOp(0x68, 8, () => this.TestBit_(this.B, 5));
      this.CbOp(0x70, 8, () => this.TestBit_(this.B, 6));
      this.CbOp(0x78, 8, () => this.TestBit_(this.B, 7));

      this.CbOp(0x41, 8, () => this.TestBit_(this.C, 0));
      this.CbOp(0x49, 8, () => this.TestBit_(this.C, 1));
      this.CbOp(0x51, 8, () => this.TestBit_(this.C, 2));
      this.CbOp(0x59, 8, () => this.TestBit_(this.C, 3));
      this.CbOp(0x61, 8, () => this.TestBit_(this.C, 4));
      this.CbOp(0x69, 8, () => this.TestBit_(this.C, 5));
      this.CbOp(0x71, 8, () => this.TestBit_(this.C, 6));
      this.CbOp(0x79, 8, () => this.TestBit_(this.C, 7));

      this.CbOp(0x42, 8, () => this.TestBit_(this.D, 0));
      this.CbOp(0x4a, 8, () => this.TestBit_(this.D, 1));
      this.CbOp(0x52, 8, () => this.TestBit_(this.D, 2));
      this.CbOp(0x5a, 8, () => this.TestBit_(this.D, 3));
      this.CbOp(0x62, 8, () => this.TestBit_(this.D, 4));
      this.CbOp(0x6a, 8, () => this.TestBit_(this.D, 5));
      this.CbOp(0x72, 8, () => this.TestBit_(this.D, 6));
      this.CbOp(0x7a, 8, () => this.TestBit_(this.D, 7));

      this.CbOp(0x43, 8, () => this.TestBit_(this.E, 0));
      this.CbOp(0x4b, 8, () => this.TestBit_(this.E, 1));
      this.CbOp(0x53, 8, () => this.TestBit_(this.E, 2));
      this.CbOp(0x5b, 8, () => this.TestBit_(this.E, 3));
      this.CbOp(0x63, 8, () => this.TestBit_(this.E, 4));
      this.CbOp(0x6b, 8, () => this.TestBit_(this.E, 5));
      this.CbOp(0x73, 8, () => this.TestBit_(this.E, 6));
      this.CbOp(0x7b, 8, () => this.TestBit_(this.E, 7));

      this.CbOp(0x44, 8, () => this.TestBit_(this.H, 0));
      this.CbOp(0x4c, 8, () => this.TestBit_(this.H, 1));
      this.CbOp(0x54, 8, () => this.TestBit_(this.H, 2));
      this.CbOp(0x5c, 8, () => this.TestBit_(this.H, 3));
      this.CbOp(0x64, 8, () => this.TestBit_(this.H, 4));
      this.CbOp(0x6c, 8, () => this.TestBit_(this.H, 5));
      this.CbOp(0x74, 8, () => this.TestBit_(this.H, 6));
      this.CbOp(0x7c, 8, () => this.TestBit_(this.H, 7));

      this.CbOp(0x45, 8, () => this.TestBit_(this.L, 0));
      this.CbOp(0x4d, 8, () => this.TestBit_(this.L, 1));
      this.CbOp(0x55, 8, () => this.TestBit_(this.L, 2));
      this.CbOp(0x5d, 8, () => this.TestBit_(this.L, 3));
      this.CbOp(0x65, 8, () => this.TestBit_(this.L, 4));
      this.CbOp(0x6d, 8, () => this.TestBit_(this.L, 5));
      this.CbOp(0x75, 8, () => this.TestBit_(this.L, 6));
      this.CbOp(0x7d, 8, () => this.TestBit_(this.L, 7));

      this.CbOp(0x46, 16, () => this.TestBit_(this._Hl_, 0));
      this.CbOp(0x4e, 16, () => this.TestBit_(this._Hl_, 1));
      this.CbOp(0x56, 16, () => this.TestBit_(this._Hl_, 2));
      this.CbOp(0x5e, 16, () => this.TestBit_(this._Hl_, 3));
      this.CbOp(0x66, 16, () => this.TestBit_(this._Hl_, 4));
      this.CbOp(0x6e, 16, () => this.TestBit_(this._Hl_, 5));
      this.CbOp(0x76, 16, () => this.TestBit_(this._Hl_, 6));
      this.CbOp(0x7e, 16, () => this.TestBit_(this._Hl_, 7));
      // 2. SET b,r
      this.CbOp(0xc7, 8, () => this.SetBit_(this.A, 0));
      this.CbOp(0xcf, 8, () => this.SetBit_(this.A, 1));
      this.CbOp(0xd7, 8, () => this.SetBit_(this.A, 2));
      this.CbOp(0xdf, 8, () => this.SetBit_(this.A, 3));
      this.CbOp(0xe7, 8, () => this.SetBit_(this.A, 4));
      this.CbOp(0xef, 8, () => this.SetBit_(this.A, 5));
      this.CbOp(0xf7, 8, () => this.SetBit_(this.A, 6));
      this.CbOp(0xff, 8, () => this.SetBit_(this.A, 7));

      this.CbOp(0xc0, 8, () => this.SetBit_(this.B, 0));
      this.CbOp(0xc8, 8, () => this.SetBit_(this.B, 1));
      this.CbOp(0xd0, 8, () => this.SetBit_(this.B, 2));
      this.CbOp(0xd8, 8, () => this.SetBit_(this.B, 3));
      this.CbOp(0xe0, 8, () => this.SetBit_(this.B, 4));
      this.CbOp(0xe8, 8, () => this.SetBit_(this.B, 5));
      this.CbOp(0xf0, 8, () => this.SetBit_(this.B, 6));
      this.CbOp(0xf8, 8, () => this.SetBit_(this.B, 7));

      this.CbOp(0xc1, 8, () => this.SetBit_(this.C, 0));
      this.CbOp(0xc9, 8, () => this.SetBit_(this.C, 1));
      this.CbOp(0xd1, 8, () => this.SetBit_(this.C, 2));
      this.CbOp(0xd9, 8, () => this.SetBit_(this.C, 3));
      this.CbOp(0xe1, 8, () => this.SetBit_(this.C, 4));
      this.CbOp(0xe9, 8, () => this.SetBit_(this.C, 5));
      this.CbOp(0xf1, 8, () => this.SetBit_(this.C, 6));
      this.CbOp(0xf9, 8, () => this.SetBit_(this.C, 7));

      this.CbOp(0xc2, 8, () => this.SetBit_(this.D, 0));
      this.CbOp(0xca, 8, () => this.SetBit_(this.D, 1));
      this.CbOp(0xd2, 8, () => this.SetBit_(this.D, 2));
      this.CbOp(0xda, 8, () => this.SetBit_(this.D, 3));
      this.CbOp(0xe2, 8, () => this.SetBit_(this.D, 4));
      this.CbOp(0xea, 8, () => this.SetBit_(this.D, 5));
      this.CbOp(0xf2, 8, () => this.SetBit_(this.D, 6));
      this.CbOp(0xfa, 8, () => this.SetBit_(this.D, 7));

      this.CbOp(0xc3, 8, () => this.SetBit_(this.E, 0));
      this.CbOp(0xcb, 8, () => this.SetBit_(this.E, 1));
      this.CbOp(0xd3, 8, () => this.SetBit_(this.E, 2));
      this.CbOp(0xdb, 8, () => this.SetBit_(this.E, 3));
      this.CbOp(0xe3, 8, () => this.SetBit_(this.E, 4));
      this.CbOp(0xeb, 8, () => this.SetBit_(this.E, 5));
      this.CbOp(0xf3, 8, () => this.SetBit_(this.E, 6));
      this.CbOp(0xfb, 8, () => this.SetBit_(this.E, 7));

      this.CbOp(0xc4, 8, () => this.SetBit_(this.H, 0));
      this.CbOp(0xcc, 8, () => this.SetBit_(this.H, 1));
      this.CbOp(0xd4, 8, () => this.SetBit_(this.H, 2));
      this.CbOp(0xdc, 8, () => this.SetBit_(this.H, 3));
      this.CbOp(0xe4, 8, () => this.SetBit_(this.H, 4));
      this.CbOp(0xec, 8, () => this.SetBit_(this.H, 5));
      this.CbOp(0xf4, 8, () => this.SetBit_(this.H, 6));
      this.CbOp(0xfc, 8, () => this.SetBit_(this.H, 7));

      this.CbOp(0xc5, 8, () => this.SetBit_(this.L, 0));
      this.CbOp(0xcd, 8, () => this.SetBit_(this.L, 1));
      this.CbOp(0xd5, 8, () => this.SetBit_(this.L, 2));
      this.CbOp(0xdd, 8, () => this.SetBit_(this.L, 3));
      this.CbOp(0xe5, 8, () => this.SetBit_(this.L, 4));
      this.CbOp(0xed, 8, () => this.SetBit_(this.L, 5));
      this.CbOp(0xf5, 8, () => this.SetBit_(this.L, 6));
      this.CbOp(0xfd, 8, () => this.SetBit_(this.L, 7));

      this.CbOp(0xc6, 16, () => this.SetBit_(this._Hl_, 0));
      this.CbOp(0xce, 16, () => this.SetBit_(this._Hl_, 1));
      this.CbOp(0xd6, 16, () => this.SetBit_(this._Hl_, 2));
      this.CbOp(0xde, 16, () => this.SetBit_(this._Hl_, 3));
      this.CbOp(0xe6, 16, () => this.SetBit_(this._Hl_, 4));
      this.CbOp(0xee, 16, () => this.SetBit_(this._Hl_, 5));
      this.CbOp(0xf6, 16, () => this.SetBit_(this._Hl_, 6));
      this.CbOp(0xfe, 16, () => this.SetBit_(this._Hl_, 7));
      // 3. RES b,r
      this.CbOp(0x87, 8, () => this.ResetBit_(this.A, 0));
      this.CbOp(0x8f, 8, () => this.ResetBit_(this.A, 1));
      this.CbOp(0x97, 8, () => this.ResetBit_(this.A, 2));
      this.CbOp(0x9f, 8, () => this.ResetBit_(this.A, 3));
      this.CbOp(0xa7, 8, () => this.ResetBit_(this.A, 4));
      this.CbOp(0xaf, 8, () => this.ResetBit_(this.A, 5));
      this.CbOp(0xb7, 8, () => this.ResetBit_(this.A, 6));
      this.CbOp(0xbf, 8, () => this.ResetBit_(this.A, 7));

      this.CbOp(0x80, 8, () => this.ResetBit_(this.B, 0));
      this.CbOp(0x88, 8, () => this.ResetBit_(this.B, 1));
      this.CbOp(0x90, 8, () => this.ResetBit_(this.B, 2));
      this.CbOp(0x98, 8, () => this.ResetBit_(this.B, 3));
      this.CbOp(0xa0, 8, () => this.ResetBit_(this.B, 4));
      this.CbOp(0xa8, 8, () => this.ResetBit_(this.B, 5));
      this.CbOp(0xb0, 8, () => this.ResetBit_(this.B, 6));
      this.CbOp(0xb8, 8, () => this.ResetBit_(this.B, 7));

      this.CbOp(0x81, 8, () => this.ResetBit_(this.C, 0));
      this.CbOp(0x89, 8, () => this.ResetBit_(this.C, 1));
      this.CbOp(0x91, 8, () => this.ResetBit_(this.C, 2));
      this.CbOp(0x99, 8, () => this.ResetBit_(this.C, 3));
      this.CbOp(0xa1, 8, () => this.ResetBit_(this.C, 4));
      this.CbOp(0xa9, 8, () => this.ResetBit_(this.C, 5));
      this.CbOp(0xb1, 8, () => this.ResetBit_(this.C, 6));
      this.CbOp(0xb9, 8, () => this.ResetBit_(this.C, 7));

      this.CbOp(0x82, 8, () => this.ResetBit_(this.D, 0));
      this.CbOp(0x8a, 8, () => this.ResetBit_(this.D, 1));
      this.CbOp(0x92, 8, () => this.ResetBit_(this.D, 2));
      this.CbOp(0x9a, 8, () => this.ResetBit_(this.D, 3));
      this.CbOp(0xa2, 8, () => this.ResetBit_(this.D, 4));
      this.CbOp(0xaa, 8, () => this.ResetBit_(this.D, 5));
      this.CbOp(0xb2, 8, () => this.ResetBit_(this.D, 6));
      this.CbOp(0xba, 8, () => this.ResetBit_(this.D, 7));

      this.CbOp(0x83, 8, () => this.ResetBit_(this.E, 0));
      this.CbOp(0x8b, 8, () => this.ResetBit_(this.E, 1));
      this.CbOp(0x93, 8, () => this.ResetBit_(this.E, 2));
      this.CbOp(0x9b, 8, () => this.ResetBit_(this.E, 3));
      this.CbOp(0xa3, 8, () => this.ResetBit_(this.E, 4));
      this.CbOp(0xab, 8, () => this.ResetBit_(this.E, 5));
      this.CbOp(0xb3, 8, () => this.ResetBit_(this.E, 6));
      this.CbOp(0xbb, 8, () => this.ResetBit_(this.E, 7));

      this.CbOp(0x84, 8, () => this.ResetBit_(this.H, 0));
      this.CbOp(0x8c, 8, () => this.ResetBit_(this.H, 1));
      this.CbOp(0x94, 8, () => this.ResetBit_(this.H, 2));
      this.CbOp(0x9c, 8, () => this.ResetBit_(this.H, 3));
      this.CbOp(0xa4, 8, () => this.ResetBit_(this.H, 4));
      this.CbOp(0xac, 8, () => this.ResetBit_(this.H, 5));
      this.CbOp(0xb4, 8, () => this.ResetBit_(this.H, 6));
      this.CbOp(0xbc, 8, () => this.ResetBit_(this.H, 7));

      this.CbOp(0x85, 8, () => this.ResetBit_(this.L, 0));
      this.CbOp(0x8d, 8, () => this.ResetBit_(this.L, 1));
      this.CbOp(0x95, 8, () => this.ResetBit_(this.L, 2));
      this.CbOp(0x9d, 8, () => this.ResetBit_(this.L, 3));
      this.CbOp(0xa5, 8, () => this.ResetBit_(this.L, 4));
      this.CbOp(0xad, 8, () => this.ResetBit_(this.L, 5));
      this.CbOp(0xb5, 8, () => this.ResetBit_(this.L, 6));
      this.CbOp(0xbd, 8, () => this.ResetBit_(this.L, 7));

      this.CbOp(0x86, 16, () => this.ResetBit_(this._Hl_, 0));
      this.CbOp(0x8e, 16, () => this.ResetBit_(this._Hl_, 1));
      this.CbOp(0x96, 16, () => this.ResetBit_(this._Hl_, 2));
      this.CbOp(0x9e, 16, () => this.ResetBit_(this._Hl_, 3));
      this.CbOp(0xa6, 16, () => this.ResetBit_(this._Hl_, 4));
      this.CbOp(0xae, 16, () => this.ResetBit_(this._Hl_, 5));
      this.CbOp(0xb6, 16, () => this.ResetBit_(this._Hl_, 6));
      this.CbOp(0xbe, 16, () => this.ResetBit_(this._Hl_, 7));
    }

    private void Define338_Jumps_() {
      // 1. JP nn
      this.Op(0xc3, 16, () => this.Pc = this.D16);
      // 2. JP cc,nn
      this.Op(0xc2,
              () => {
                var jumpAddress = this.D16;
                if (!this.Registers.ZFlag) {
                  this.Pc = jumpAddress;
                  return 16;
                }
                return 12;
              });
      this.Op(0xca,
              () => {
                var jumpAddress = this.D16;
                if (this.Registers.ZFlag) {
                  this.Pc = jumpAddress;
                  return 16;
                }
                return 12;
              });
      this.Op(0xd2,
              () => {
                var jumpAddress = this.D16;
                if (!this.Registers.CFlag) {
                  this.Pc = jumpAddress;
                  return 16;
                }
                return 12;
              });
      this.Op(0xda,
              () => {
                var jumpAddress = this.D16;
                if (this.Registers.CFlag) {
                  this.Pc = jumpAddress;
                  return 16;
                }
                return 12;
              });
      // 3. JP (HL)
      this.Op(0xe9, 4, () => this.Pc = this.Hl.Value);
      // 4. JR n
      this.Op(0x18,
              12,
              () => {
                var r8 = this.R8;
                this.Pc = (ushort) (this.Pc + r8);
              });
      // 5. JR cc,n
      this.Op(0x20,
              () => {
                var r8 = this.R8;
                if (!this.Registers.ZFlag) {
                  this.Pc = (ushort) (this.Pc + r8);
                  return 12;
                }
                return 8;
              });
      this.Op(0x28,
              () => {
                var r8 = this.R8;
                if (this.Registers.ZFlag) {
                  this.Pc = (ushort) (this.Pc + r8);
                  return 12;
                }
                return 8;
              });
      this.Op(0x30,
              () => {
                var r8 = this.R8;
                if (!this.Registers.CFlag) {
                  this.Pc = (ushort) (this.Pc + r8);
                  return 12;
                }
                return 8;
              });
      this.Op(0x38,
              () => {
                var r8 = this.R8;
                if (this.Registers.CFlag) {
                  this.Pc = (ushort) (this.Pc + r8);
                  return 12;
                }
                return 8;
              });
    }

    private void Call_() => this.CallIf_(true);

    private bool CallIf_(bool ifCondition) {
      var callAddress = this.D16;
      var nextInstruction = this.Pc;
      if (ifCondition) {
        this.Stack.Push16(nextInstruction);
        this.Pc = callAddress;
        return true;
      }
      return false;
    }

    private void Define339_Calls_() {
      // 1. CALL nn
      this.Op(0xcd, 24, () => this.Call_());
      // 1. CALL cc,nn
      this.Op(0xc4,
              () => {
                if (this.CallIf_(!this.Registers.ZFlag)) {
                  return 24;
                }
                return 12;
              });
      this.Op(0xcc,
              () => {
                if (this.CallIf_(this.Registers.ZFlag)) {
                  return 24;
                }
                return 12;
              });
      this.Op(0xd4,
              () => {
                if (this.CallIf_(!this.Registers.CFlag)) {
                  return 24;
                }
                return 12;
              });
      this.Op(0xdc,
              () => {
                if (this.CallIf_(this.Registers.CFlag)) {
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
                this.Stack.Push16(this.Pc);
                this.Pc = 0x00;
              });
      this.Op(0xcf,
              16,
              () => {
                this.Stack.Push16(this.Pc);
                this.Pc = 0x08;
              });
      this.Op(0xd7,
              16,
              () => {
                this.Stack.Push16(this.Pc);
                this.Pc = 0x10;
              });
      this.Op(0xdf,
              16,
              () => {
                this.Stack.Push16(this.Pc);
                this.Pc = 0x18;
              });
      this.Op(0xe7,
              16,
              () => {
                this.Stack.Push16(this.Pc);
                this.Pc = 0x20;
              });
      this.Op(0xef,
              16,
              () => {
                this.Stack.Push16(this.Pc);
                this.Pc = 0x28;
              });
      this.Op(0xf7,
              16,
              () => {
                this.Stack.Push16(this.Pc);
                this.Pc = 0x30;
              });
      this.Op(0xff,
              16,
              () => {
                this.Stack.Push16(this.Pc);
                this.Pc = 0x38;
              });
    }

    private void Define3311_Returns_() {
      // 1. RET
      this.Op(0xc9, 16, () => this.Pc = this.Stack.Pop16());
      // 2. RET cc
      this.Op(0xc0,
              () => {
                if (!this.Registers.ZFlag) {
                  this.Pc = this.Stack.Pop16();
                  return 20;
                }
                return 8;
              });
      this.Op(0xc8,
              () => {
                if (this.Registers.ZFlag) {
                  this.Pc = this.Stack.Pop16();
                  return 20;
                }
                return 8;
              });
      this.Op(0xd0,
              () => {
                if (!this.Registers.CFlag) {
                  this.Pc = this.Stack.Pop16();
                  return 20;
                }
                return 8;
              });
      this.Op(0xd8,
              () => {
                if (this.Registers.CFlag) {
                  this.Pc = this.Stack.Pop16();
                  return 20;
                }
                return 8;
              });
      // 3. RETI
      this.Op(0xd9,
              16,
              () => {
                this.Pc = this.Stack.Pop16();
                this.InterruptsState = InterruptsState.ON;
              });
    }
  }
}