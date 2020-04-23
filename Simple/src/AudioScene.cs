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
      //private readonly AudioStreamSourceOpenTk audioStreamSource_;
      private readonly IAudioSource source_;
      private readonly IAudioBuffer buffer_;
      private readonly IPcmData pcm_;

      public AudioComponent(IAudio audio) {
        var factory = audio.Factory;
        this.source_ = factory.NewAudioSource();
        this.buffer_ = factory.NewAudioBuffer();

        this.pcm_ =
            new OggLoader().Load(
                LocalFile.WithinResources("music/lobbyBackAndForth.ogg"));
        this.buffer_.FillWithPcm(this.pcm_);

        Logger.Log(LogType.DEBUG, LogSeverity.INFO, "" + this.pcm_.Channels);
        Logger.Log(LogType.DEBUG, LogSeverity.INFO, "" + this.pcm_.SampleRate);
        Logger.Log(LogType.DEBUG,
                   LogSeverity.INFO,
                   "" + this.pcm_.BytesPerSample);
        Logger.Log(LogType.DEBUG, LogSeverity.INFO, "" + this.pcm_.Pcm.Length);

        Logger.Log(LogType.DEBUG, LogSeverity.INFO, "---");

        /*for (var i = 0; i < 10000; ++i) {
          Logger.Log(LogType.DEBUG, LogSeverity.INFO, "" + this.pcm_.Pcm[i]);
        }*/

        this.source_.Play(this.buffer_, true);
      }

      protected override void Discard() {}

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