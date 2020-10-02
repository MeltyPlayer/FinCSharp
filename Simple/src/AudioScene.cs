using fin.audio;
using fin.graphics;

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
using fin.math.number;

namespace simple {
  public interface IStreamPopulator {
    void Populate(byte[] bytes);
    float SampleOffsetFraction { get; }
  }

  public class ForwardStreamPopulator : IStreamPopulator {
    private readonly IPcmData pcm_;
    private int position_ = 0;

    public ForwardStreamPopulator(IPcmData pcm) {
      this.pcm_ = pcm;
    }

    public void Populate(byte[] bytes) {
      var pcm = this.pcm_.Pcm;
      for (var i = 0;
           i < bytes.Length;
           ++i) {
        bytes[i] =
            pcm[this.position_];
        if (++this.position_ >
            pcm.Length) {
          this.position_ = 0;
        }
      }
    }

    public float SampleOffsetFraction =>
        1f * this.position_ / this.pcm_.Pcm.Length;
  }

  public class RateStreamPopulator : IStreamPopulator {
    private readonly IPcmData pcm_;
    private CircularRangedNumber<float> sampleOffset_;

    private float rate_ = .25f;

    public RateStreamPopulator(IPcmData pcm) {
      this.pcm_ = pcm;
      this.sampleOffset_ =
          new CircularRangedNumber<float>(0, 0, this.pcm_.SampleCount);
    }

    public void Populate(byte[] bytes) {
      var bytesEach = this.pcm_.BytesPerSample * this.pcm_.Channels;
      var samples = (int) (1f * bytes.Length / bytesEach);


      var pcm = this.pcm_.Pcm;
      for (var i = 0; i < samples; ++i) {
        var position = (int) Math.Floor(this.sampleOffset_.Value);

        for (var ii = 0; ii < bytesEach; ++ii) {
          bytes[i * bytesEach + ii] = pcm[position * bytesEach + ii];
        }

        this.sampleOffset_.Value += this.rate_;
      }
    }

    public float SampleOffsetFraction =>
        this.sampleOffset_.Value / this.pcm_.SampleCount;
  }

  public sealed class AudioScene : BScene {
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