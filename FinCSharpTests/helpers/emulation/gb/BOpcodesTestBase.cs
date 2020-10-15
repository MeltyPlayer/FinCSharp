using fin.emulation.gb;
using fin.emulation.gb.memory;
using fin.helpers.emulation.gb.memory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers.emulation.gb {
  public abstract class BOpcodesTestBase {
    public ISerialBus SerialBus { get; private set; }
    public ReplayRegisters ReplayRegisters { get; private set; }
    public Memory Memory { get; private set; }
    public ReplayOpcodes Opcodes { get; private set; }
    private Cpu cpu_;


    public ReplayDoubleRegister Bc => this.ReplayRegisters.Bc_R;
    public ReplayDoubleRegister De => this.ReplayRegisters.De_R;
    public ReplayDoubleRegister Hl => this.ReplayRegisters.Hl_R;
    public ReplayDoubleRegister Sp => this.ReplayRegisters.Sp_R;

    public ReplaySingleRegister _Hl_ {
      get {
        {
          var hl_R = this.ReplayRegisters.Hl_R;
          hl_R.LockState = LockState.CAN_READ_AND_WRITE;
          hl_R.Value = 0x8000;
          hl_R.LockState = LockState.CAN_READ;

          var _hl_ =
              new RamRegister(this.Memory.MemoryMap, hl_R);
          return new ReplaySingleRegister(
              "(HL)",
              0,
              LockState.LOCKED,
              _hl_);
        }
      }
    }


    [TestInitialize]
    public void BeforeEach() {
      var lcd = new Lcd();
      this.SerialBus = new SerialBus();
      var ioAddresses = new IoAddresses(this.SerialBus);
      this.ReplayRegisters = new ReplayRegisters();
      this.Memory = new Memory(new MemoryMap(ioAddresses), this.ReplayRegisters);
      this.Opcodes = new ReplayOpcodes(this.Memory);
      this.cpu_ = new Cpu(lcd, this.Memory, this.Opcodes);
    }

    public void Reset() {
      this.cpu_.Reset();
      this.Memory.Reset();
    }

    public int ExecuteInOrder(int instructionCount, params byte[] ops) {
      this.Opcodes.ResetReplay();

      var pcAddress = this.ReplayRegisters.Pc.Value;

      var opLength = ops.Length;
      var data = new byte[pcAddress + opLength];
      for (var i = 0; i < opLength; ++i) {
        var op = ops[i];
        data[pcAddress + i] = op;
        this.Opcodes.NextInReplay(op);
      }

      this.Memory.MemoryMap.Rom = new Rom(data);
      return this.cpu_.ExecuteInstructions(instructionCount);
    }
  }
}