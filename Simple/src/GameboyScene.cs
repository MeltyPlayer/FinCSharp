using fin.audio;
using fin.graphics;

using System;

using fin.app.scene;
using fin.app.window;
using fin.emulation.gb;
using fin.settings;
using fin.math.geometry;
using fin.io;

namespace simple {
  public sealed class GameboyScene : BScene {
    protected override void Init(SceneInitTickEvent evt) {
      var settings = Settings.Load();
      var appWidth = settings.Resolution.Width;
      var appHeight = settings.Resolution.Height;

      var windows =
          evt.App.WindowManager.InitWindowsForScene(
              new WindowArgs().SetDimensions(settings.Resolution));

      var window = windows[0];
      window.Width = appWidth;
      window.Height = appHeight;
      window.Visible = true;

      var app = evt.App;
      var instantiator = app.Instantiator;
      var viewRoot = instantiator.NewTopLevelChild();

      var view =
          window.NewView(
              new MutableBoundingBox<int>(0, 0, appWidth, appHeight));
      view.AddOrthographicCamera(viewRoot);

      // Add contents of view.
      var gameboy =
          instantiator.Wrap(viewRoot, new GameboyComponent(app));
      gameboy.LaunchRom(LocalIo.Resources.GetFile("sml.gb"));

      /*var romPath =
          "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/" +
          "cpu_instrs" +
          ".gb";
      var romFile = LocalFile.At(romPath);
      gameboy.LaunchRom(romFile);*/
    }
  }
}