using fin.audio;
using fin.graphics;

using System;

using fin.app;
using fin.app.events;
using fin.app.node;
using fin.app.scene;
using fin.app.window;
using fin.emulation.gb;
using fin.graphics.camera;
using fin.graphics.color;
using fin.settings;
using fin.math.geometry;
using fin.file;

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
      gameboy.LaunchRom(LocalFile.WithinResources("sml.gb"));

      /*var romPath =
          "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/" +
          "cpu_instrs" +
          ".gb";
      var romFile = LocalFile.At(romPath);
      gameboy.LaunchRom(romFile);*/
    }

    private class AudioComponent : IComponent {
      private readonly IAudioStreamSource source_;

      //private readonly IAudioSource source_;
      private readonly IAudioBuffer buffer_;
      private readonly IPcmData pcm_;

      private IStreamPopulator
          streamPopulator_;

      public AudioComponent(IAudio audio) {
        this.pcm_ =
            new OggLoader().Load(
                LocalFile.WithinResources("music/lobbyBackAndForth.ogg"));

        this.streamPopulator_ = new RateStreamPopulator(this.pcm_);

        var factory = audio.Factory;

        //this.source_ = factory.NewAudioSource();
        //this.buffer_ = factory.NewAudioBuffer();

        //this.buffer_.FillWithPcm(this.pcm_);

        //this.source_.Play(this.buffer_, true);

        this.source_ = factory.NewAudioStreamSource(
            this.streamPopulator_.Populate,
            this.pcm_.Channels,
            this.pcm_.BytesPerSample,
            this.pcm_.SampleRate);
        this.source_.Play(true);
      }

      [OnTick]
      private void StartTick_(StartTickEvent _) {
        //this.streamSource_.PollForProcessedBuffers();
      }

      [OnTick]
      private void RenderForOrthographicCamera_(
          RenderForOrthographicCameraTickEvent evt) {
        var g = evt.Graphics;

        this.RenderTotalWaveform_(g);
      }

      private static int WAVEFORM_HEIGHT = 200;

      private IBoundingBox<int> left_ =
          new MutableBoundingBox<int>(0,
                                      480 / 4 -
                                      AudioComponent.WAVEFORM_HEIGHT /
                                      2,
                                      640,
                                      AudioComponent.WAVEFORM_HEIGHT);

      private IBoundingBox<int> right_ =
          new MutableBoundingBox<int>(0,
                                      480 * 3 / 4 -
                                      AudioComponent.WAVEFORM_HEIGHT /
                                      2,
                                      640,
                                      AudioComponent.WAVEFORM_HEIGHT);

      private void RenderTotalWaveform_(IGraphics g) {
        this.RenderTotalWaveformForChannel_(g, this.left_);
        this.RenderTotalWaveformForChannel_(g, this.right_, true);

        var p = g.Primitives;
        var r2d = g.Render2d;

        //var sampleOffsetFraction = this.source_.SampleOffsetFraction;
        float? sampleOffsetFraction =
            this.streamPopulator_.SampleOffsetFraction;

        var cursorX = 640 * sampleOffsetFraction ?? 0;

        p.VertexColor(ColorConstants.WHITE);
        r2d.Line(new Vertex2d(cursorX, 0), new Vertex2d(cursorX, 480));
      }

      private void RenderTotalWaveformForChannel_(
          IGraphics g,
          IBoundingBox<int> boundingBox,
          bool rightSide = false) {
        var p = g.Primitives;
        var r2d = g.Render2d;

        var leftX = boundingBox.LeftX;
        var rightX = boundingBox.RightX;
        var width = boundingBox.Dimensions.Width;

        var cY = boundingBox.CenterY;
        var maxAmplitude = boundingBox.Dimensions.Height * .5f;

        p.VertexColor(ColorConstants.GREEN);
        r2d.Line(new Vertex2d(leftX, cY), new Vertex2d(rightX, cY));

        p.Begin(PrimitiveType.LINE_STRIP);
        for (var x = leftX; x < leftX + rightX; ++x) {
          var i = (int) Math.Floor((1f * x) / width * this.pcm_.SampleCount);

          var amplitude = !rightSide
                              ? this.pcm_.GetLeftAmplitude(i)
                              : this.pcm_.GetRightAmplitude(i);

          p.VertexColor(ColorConstants.RED);
          p.Vertex(x, cY + maxAmplitude * amplitude);
        }
        p.End();
      }
    }
  }
}