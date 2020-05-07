using fin.audio;
using fin.audio.impl.opentk;
using fin.graphics;
using fin.log;

namespace simple {
  using System;
  using System.Drawing;

  using fin.app;
  using fin.app.events;
  using fin.app.node;
  using fin.app.scene;
  using fin.app.window;
  using fin.graphics.camera;
  using fin.graphics.color;
  using fin.input;
  using fin.input.button;
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
      //private readonly IAudioSource source_;
      //private readonly IAudioBuffer buffer_;
      private readonly IPcmData pcm_;

      private int position_ = 0;

      public AudioComponent(IAudio audio) {
        this.pcm_ =
            new OggLoader().Load(
                LocalFile.WithinResources("music/lobby.ogg"));

        var factory = audio.Factory;

        /*this.source_ = factory.NewAudioSource();
        this.buffer_ = factory.NewAudioBuffer();

        this.buffer_.FillWithPcm(this.pcm_);

        this.source_.Play(this.buffer_, true);*/

        this.streamSource_ = factory.NewAudioStreamSource(bytes => {
          var pcm = this.pcm_.Pcm;
          for (var i = 0; i < bytes.Length; ++i) {
            bytes[i] = pcm[this.position_];
            if (++this.position_ > pcm.Length) {
              this.position_ = 0;
            }
          }
        }, this.pcm_.SampleRate);
        this.streamSource_.Play(true);
      }

      protected override void Discard() {}

      [OnTick]
      private void StartTick_(StartTickEvent _) {
        this.streamSource_.PollForProcessedBuffers();
      }

      [OnTick]
      private void RenderForOrthographicCamera_(
          RenderForOrthographicCameraTickEvent evt) {
        var p = evt.Graphics.Primitives;
        var bytes = this.pcm_.Pcm;

        var cY = 480 / 2;

        p.Begin(PrimitiveType.LINE_STRIP);
        for (var x = 0; x < 640; ++x) {
          var i = (int) Math.Round((1f * x) / 640 * bytes.Length);

          p.VertexColor(ColorConstants.RED);
          p.Vertex(x, cY + bytes[i]);
        }
        p.End();

        p.VertexColor(ColorConstants.GREEN);
        evt.Graphics.Render2d.Line(new Vertex2d(0, cY), new Vertex2d(640, cY));
      }
    }
  }
}