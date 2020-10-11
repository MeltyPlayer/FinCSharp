using fin.app;
using fin.app.events;
using fin.app.node;
using fin.file;
using fin.graphics.camera;

namespace fin.emulation.gb {
  public class GameboyComponent : IComponent {
    private readonly Cpu cpu_ = new Cpu();

    public void LaunchRom(IFile rom) {

    }

    // TODO: Consider hooking into a different event.
    [OnTick]
    private void StartTick_(StartTickEvent _) {
      this.cpu_.Tick(70221);
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      // TODO: Update
    }
  }
}
