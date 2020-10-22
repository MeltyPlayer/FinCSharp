using System.IO;
using System.Text;

using fin.app;
using fin.app.events;
using fin.app.node;
using fin.emulation.gb.memory;
using fin.emulation.gb.memory.io;
using fin.file;
using fin.graphics.camera;
using fin.graphics.color;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Color = System.Drawing.Color;

namespace fin.emulation.gb {
  public class GameboyComponent : IComponent {
    private readonly Lcd lcd_;
    private readonly Mmu mmu_;
    private readonly Cpu cpu_;

    public GameboyComponent() {
      this.lcd_ = new Lcd();
      this.mmu_ =
          new Mmu(new MemoryMap(new IoAddresses(new SerialBus())),
                     new Registers());
      var opcodes = new Opcodes(this.mmu_);
      this.cpu_ = new Cpu(this.lcd_, this.mmu_, opcodes);

      var outputPath =
          "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/output.txt";
      this.writer_ = new StreamWriter(outputPath);
    }

    public void LaunchRom(IFile romFile) {
      this.cpu_.Reset();
      this.mmu_.Reset();

      var bytes = LocalFileUtil.ReadBytes((romFile as LocalFile)!);
      this.mmu_.MemoryMap.Rom = new Rom(bytes);
    }

    private readonly StreamWriter writer_;

    // TODO: Consider hooking into a different event.
    [OnTick]
    private void StartTick_(StartTickEvent _) {
      var doLog = false;

      var cpu = this.cpu_;
      var memoryMap = this.mmu_.MemoryMap;
      var ioAddresses = memoryMap.IoAddresses;

      if (!doLog) {
        this.cpu_.ExecuteCycles(70224); // 70221
      } else {
        var cyclesPerTick = 1000;

        var registers = this.mmu_.Registers;

        var pc = registers.Pc;
        ushort initialPc = 0;

        var lcdc = ioAddresses.Lcdc;
        var ly = ioAddresses.Ly;

        var instruction = 0;
        var output = new StringBuilder();

        try {
          for (;;) {
            var ppuMode = cpu.PpuMode;
            initialPc = pc.Value;

            output.AppendFormat("0x{0:x4}:  ", initialPc);
            output.AppendFormat("{0:x2} |", memoryMap[initialPc]);
            output.AppendFormat(" af={0:x4}", registers.Af.Value);
            output.AppendFormat(" bc={0:x4}", registers.Bc.Value);
            output.AppendFormat(" de={0:x4}", registers.De.Value);
            output.AppendFormat(" hl={0:x4}", registers.Hl.Value);
            output.AppendFormat(" sp={0:x4}", registers.Sp.Value);
            output.AppendFormat(" pc={0:x4} |", registers.Pc.Value);

            output.AppendFormat(" scl={0}", cpu.UpwardScanlineCycleCounter);
            output.AppendFormat(" st={0}", cpu.PpuModeCycleCount);
            output.AppendFormat(" cnt={0} |", cpu.ScanlineCycleCounter / 2);

            output.AppendFormat(" lcdc={0:x2}", lcdc.Value);
            output.AppendFormat(" ly={0:x2}", ly.Value);
            output.AppendFormat(" ppu={0:x1} |", ppuMode);

            output.AppendFormat(" div={0:x2}", ioAddresses.Div);
            output.AppendFormat(" tima={0:x2}", ioAddresses.Tima);
            output.AppendFormat(" tma={0:x2}", ioAddresses.Tma);
            output.AppendFormat(" tac={0:x2}", ioAddresses.Tac);

            output.Append('\n');

            var cycles = cpu.ExecuteInstructions(1);

            /*if (initialPc == 0x01c6) {
              Assert.Fail("How did we get here???");
            }*/

            cyclesPerTick -= cycles;
            if (cyclesPerTick <= 0) {
              break;
            }
          }
        } finally {
          this.writer_.Write(output.ToString());
        }
      }

      ioAddresses.Joypad.Update(cpu);
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      var g = evt.Graphics;

      this.lcd_.Render(g);

      g.Primitives.VertexColor(Color.White);

      var registers = this.mmu_.Registers;
      var ioAddresses = this.mmu_.MemoryMap.IoAddresses;

      var t = g.Text;
      var i = 0;
      t.Draw(320, 20*i++, 16, 16, "pc: " + ByteFormatter.ToHex16(registers.Pc.Value));

      i++;
      t.Draw(320, 20 * i++, 16, 16, "af: " + ByteFormatter.ToHex16(registers.Af.Value));
      t.Draw(320, 20 * i++, 16, 16, "bc: " + ByteFormatter.ToHex16(registers.Bc.Value));
      t.Draw(320, 20 * i++, 16, 16, "de: " + ByteFormatter.ToHex16(registers.De.Value));
      t.Draw(320, 20 * i++, 16, 16, "hl: " + ByteFormatter.ToHex16(registers.Hl.Value));

      i++;
      t.Draw(320, 20 * i++, 16, 16, "ppu: " + ByteFormatter.ToHex8((byte)this.cpu_.PpuMode));
      t.Draw(320, 20 * i++, 16, 16, "ly: " + ByteFormatter.ToHex8(this.mmu_.MemoryMap.IoAddresses.Ly.Value));
      t.Draw(320, 20 * i++, 16, 16, ByteFormatter.ToHex8(ioAddresses.Lcdc.Value));
      t.Draw(320, 20 * i++, 16, 16, ByteFormatter.ToHex8(this.cpu_.ScanlineLcdc));
    }
  }
}