using fin.app;
using fin.app.events;
using fin.app.node;
using fin.emulation.gb.memory;
using fin.file;
using fin.graphics.camera;

namespace fin.emulation.gb {
  public class GameboyComponent : IComponent {
    private readonly Lcd lcd_;
    private readonly Memory memory_;
    private readonly Cpu cpu_;

    public GameboyComponent() {
      this.lcd_ = new Lcd();
      this.memory_ =
          new Memory(new MemoryMap(new IoAddresses(new SerialBus())), new Registers());
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
      this.cpu_.ExecuteCycles(70221);
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      this.lcd_.Render(evt.Graphics);
    }
  }
}