using fin.app;
using fin.app.events;
using fin.app.node;
using fin.emulation.gb.memory;
using fin.emulation.gb.memory.io;
using fin.file;
using fin.graphics.camera;
using fin.graphics.color;

using Color = System.Drawing.Color;

namespace fin.emulation.gb {
  public class GameboyComponent : IComponent {
    private readonly Lcd lcd_;
    private readonly Mmu memory_;
    private readonly Cpu cpu_;

    public GameboyComponent() {
      this.lcd_ = new Lcd();
      this.memory_ =
          new Mmu(new MemoryMap(new IoAddresses(new SerialBus())),
                     new Registers());
      var opcodes = new Opcodes(this.memory_);
      this.cpu_ = new Cpu(this.lcd_, this.memory_, opcodes);
    }

    public void LaunchRom(IFile romFile) {
      var bytes = LocalFileUtil.ReadBytes((romFile as LocalFile)!);
      this.memory_.MemoryMap.Rom = new Rom(bytes);
    }

    // TODO: Consider hooking into a different event.
    [OnTick]
    private void StartTick_(StartTickEvent _) {
      this.cpu_.ExecuteCycles(70224); // 70221
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      var g = evt.Graphics;

      this.lcd_.Render(g);

      g.Primitives.VertexColor(Color.White);
      var ioAddresses = this.memory_.MemoryMap.IoAddresses;
      var t = g.Text;
      t.Draw(320, 0, 16, 16, "ppu: " + ByteFormatter.ToHex8((byte) this.cpu_.PpuMode));
      t.Draw(320, 20, 16, 16, ByteFormatter.ToHex8(ioAddresses.Lcdc.Value));
      t.Draw(320, 40, 16, 16, ByteFormatter.ToHex8(this.cpu_.ScanlineLcdc));
    }
  }
}