using System.IO;
using System.Text;

using fin.app;
using fin.app.events;
using fin.app.node;
using fin.emulation.gb.memory;
using fin.emulation.gb.memory.io;
using fin.io;
using fin.graphics;
using fin.graphics.camera;
using fin.input.gamepad;

using Color = System.Drawing.Color;

namespace fin.emulation.gb {
  public class GameboyComponent : IComponent {
    private readonly Lcd lcd_;
    private readonly Mmu mmu_;
    private readonly Cpu cpu_;
    private readonly Apu apu_;

    private readonly IGamepad gamepad_;

    private readonly IPixelBufferObject pbo_;
    private bool pboCacheDirty_ = true;

    public GameboyComponent(IApp app) {
      this.gamepad_ = app.Input.Controller;

      this.lcd_ = new Lcd();
      this.mmu_ =
          new Mmu(new MemoryMap(new IoAddresses(new SerialBus())),
                  new Registers());
      var opcodes = new Opcodes(this.mmu_);

      this.cpu_ = new Cpu(this.lcd_, this.mmu_, opcodes);
      this.cpu_.OnEnterVblank += () => {
        if (!this.pboCacheDirty_) {
          return;
        }
        this.pbo_.SetAllPixels(this.lcd_.PixelData);
        this.pboCacheDirty_ = false;
      };

      this.apu_ = new Apu();
      app.Audio.Factory.NewAudioStreamSource(this.apu_.BufferSubject,
                                             2,
                                             1,
                                             Apu.FREQUENCY,
                                             10,
                                             Apu.FREQUENCY);

      var outputPath =
          "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/output.txt";
      this.writer_ = new StreamWriter(outputPath);

      this.pbo_ = app.Graphics.Textures.Create(201, 144);
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
      this.pboCacheDirty_ = true;

      var doLog = false;

      var cpu = this.cpu_;
      var memoryMap = this.mmu_.MemoryMap;
      var ioAddresses = memoryMap.IoAddresses;
      var stat = ioAddresses.Stat;

      var initialPpuMode = stat.Mode;

      int cyclesThisIteration = 0;
      if (!doLog) {
        cyclesThisIteration = this.cpu_.ExecuteCycles(70224); // 70221
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

            output.AppendFormat(" div={0:x2}", ioAddresses.Div.Value);
            output.AppendFormat(" tima={0:x2}", ioAddresses.Tima.Value);
            output.AppendFormat(" tma={0:x2}", ioAddresses.Tma.Value);
            output.AppendFormat(" tac={0:x2}", ioAddresses.Tac.Value);

            output.Append('\n');

            var cycles = cpu.ExecuteInstructions(1);
            cyclesThisIteration += cycles;

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

      this.apu_.Tick(cyclesThisIteration);
      ioAddresses.Joypad.Update(cpu);
    }


    [OnTick]
    private void ProcessInputs_(ProcessInputsEvent _) {
      var joypad = this.mmu_.MemoryMap.IoAddresses.Joypad;

      var dpad = this.gamepad_[AnalogStickType.PRIMARY].RawAxes;
      var (dpadX, dpadY) = (dpad.X, dpad.Y);
      joypad.ToggleDpadRight(dpadX > .5);
      joypad.ToggleDpadLeft(dpadX < -.5);
      joypad.ToggleDpadUp(dpadY > .5);
      joypad.ToggleDpadDown(dpadY < -.5);

      var buttonA = this.gamepad_[FaceButtonType.PRIMARY];
      var buttonB = this.gamepad_[FaceButtonType.SECONDARY];
      var buttonSelect = this.gamepad_[FaceButtonType.SELECT];
      var buttonStart = this.gamepad_[FaceButtonType.START];
      joypad.ToggleButtonA(buttonA.IsDown);
      joypad.ToggleButtonB(buttonB.IsDown);
      joypad.ToggleButtonSelect(buttonSelect.IsDown);
      joypad.ToggleButtonStart(buttonStart.IsDown);
    }


    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      var g = evt.Graphics;

      this.RenderLcd_(g);

      g.Primitives.VertexColor(Color.White);

      var registers = this.mmu_.Registers;
      var memoryMap = this.mmu_.MemoryMap;
      var ioAddresses = memoryMap.IoAddresses;

      var lX = 202;
      var bY = 145;

      var t = g.Text;
      var i = 0;
      t.Draw(lX,
             20 * i++,
             16,
             16,
             "pc: " + ByteFormatter.ToHex16(registers.Pc.Value));

      i++;
      t.Draw(lX,
             20 * i++,
             16,
             16,
             "af: " + ByteFormatter.ToHex16(registers.Af.Value));
      t.Draw(lX,
             20 * i++,
             16,
             16,
             "bc: " + ByteFormatter.ToHex16(registers.Bc.Value));
      t.Draw(lX,
             20 * i++,
             16,
             16,
             "de: " + ByteFormatter.ToHex16(registers.De.Value));
      t.Draw(lX,
             20 * i++,
             16,
             16,
             "hl: " + ByteFormatter.ToHex16(registers.Hl.Value));
      i++;

      lX = 320;
      i = 0;

      t.Draw(lX,
             20 * i++,
             16,
             16,
             "ppu: " + ByteFormatter.ToHex8((byte) this.cpu_.PpuMode));
      t.Draw(lX,
             20 * i++,
             16,
             16,
             "ly: " +
             ByteFormatter.ToHex8(this.mmu_.MemoryMap.IoAddresses.Ly.Value));
      t.Draw(lX,
             20 * i++,
             16,
             16,
             "dma: " +
             ByteFormatter.ToHex16(this.mmu_.MemoryMap.IoAddresses
                                       .LastDmaAddress));
      t.Draw(lX,
             20 * i++,
             16,
             16,
             ByteFormatter.ToHex8(ioAddresses.Lcdc.Value));
      t.Draw(lX,
             20 * i++,
             16,
             16,
             ByteFormatter.ToHex8(this.cpu_.ScanlineLcdc));

      var oam = memoryMap.Oam;

      for (i = 0; i < 40; i++) {
        var oamAddress = (ushort) (i * 4);
        var y = (byte) (oam[oamAddress] - 16);
        var x = (byte) (oam[(ushort) (oamAddress + 1)] - 8);

        var nPerRow = 7;
        var c = i % nPerRow;
        var r = (i - c) / nPerRow;

        t.Draw(c * 90,
               bY + r * 20,
               16,
               16,
               "(" +
               ByteFormatter.ToHex8(x) +
               ", " +
               ByteFormatter.ToHex8(y) +
               ")");
      }
    }

    private void RenderLcd_(IGraphics g) {
      if (!this.lcd_.Active) {
        return;
      }

      var r2d = g.Render2d;

      this.pbo_.Bind();
      r2d.Rectangle(0, 0, 201, 144, true);
      this.pbo_.Unbind();

      /*var p = g.Primitives;
      var size = 1;
      p.Begin(PrimitiveType.POINTS);
      for (var y = 0; y < height; ++y) {
        for (var x = 0; x < width; ++x) {
          var color = this.pixels_[x, y];
          p.VertexColor(color).Vertex(x, y);
          //r2d.Rectangle(x * size, y * size, size, size, true);
        }
      }
      p.End();*/
    }
  }
}