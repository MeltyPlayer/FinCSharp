using simple.platformer;

namespace simple {
  using fin.app.impl.opentk;

  public class EntryPoint {
    public static void Main() {
      var app = new AppOpenTk();
      //app.Launch(new AudioScene());
      app.Launch(new GameboyScene());
      //app.Launch(new PlatformScene());
      //app.Launch(new TestScene());
    }
  }
}