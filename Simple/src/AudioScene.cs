using fin.audio;
using fin.audio.impl.opentk;
using fin.graphics;
using fin.log;

namespace simple {
  using System;

  using fin.app;
  using fin.app.events;
  using fin.app.node;
  using fin.app.scene;
  using fin.app.window;
  using fin.graphics.camera;
  using fin.graphics.color;
  using fin.settings;
  using fin.math.geometry;
  using fin.file;

  public sealed class AudioScene : BScene {
    protected override void Discard() {}

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

      var instantiator = evt.App.Instantiator;
      var viewRoot = instantiator.NewTopLevelChild();

      var view =
          window.NewView(
              new MutableBoundingBox<int>(0, 0, appWidth, appHeight));
      view.AddOrthographicCamera(viewRoot);

      // Add contents of view.
      instantiator.NewChild(viewRoot, new AudioComponent(evt.App.Audio));
    }

    private class AudioComponent : BComponent {
      private readonly IAudioStreamSource streamSource_;
      private readonly IAudioSource source_;
      private readonly IAudioBuffer buffer_;
      private readonly IPcmData pcm_;

      private int position_ = 0;

      public AudioComponent(IAudio audio) {
        this.pcm_ =
            new OggLoader().Load(
                LocalFile.WithinResources("music/lobbyBackAndForth.ogg"));

        var factory = audio.Factory;

        this.source_ = factory.NewAudioSource();
        this.buffer_ = factory.NewAudioBuffer();

        this.buffer_.FillWithPcm(this.pcm_);

        this.source_.Play(this.buffer_, true);

        /*this.streamSource_ = factory.NewAudioStreamSource(bytes => {
          var pcm = this.pcm_.Pcm;
          for (var i = 0; i < bytes.Length; ++i) {
            bytes[i] = pcm[this.position_];
            if (++this.position_ > pcm.Length) {
              this.position_ = 0;
            }
          }
        }, this.pcm_.SampleRate);
        this.streamSource_.Play(true);*/
      }

      protected override void Discard() {}

      [OnTick]
      private void StartTick_(StartTickEvent _) {
        //this.streamSource_.PollForProcessedBuffers();
      }

      [OnTick]
      private void RenderForOrthographicCamera_(
          RenderForOrthographicCameraTickEvent evt) {
        var g = evt.Graphics;

        this.RenderWaveform_(g, this.left_);
        this.RenderWaveform_(g, this.right_, true);

        var p = g.Primitives;
        var r2d = g.Render2d;

        var cursorX = 640 * this.source_.SampleOffsetFraction ?? 0;

        p.VertexColor(ColorConstants.WHITE);
        r2d.Line(new Vertex2d(cursorX, 0), new Vertex2d(cursorX, 480));
      }

      private static int WAVEFORM_HEIGHT = 200;

      private IBoundingBox<int> left_ =
          new MutableBoundingBox<int>(0,
                                      480 / 4 - AudioComponent.WAVEFORM_HEIGHT /
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

      private void RenderWaveform_(
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