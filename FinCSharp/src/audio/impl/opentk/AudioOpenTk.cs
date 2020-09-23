using System;
using System.Collections.Generic;

using fin.discardable;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace fin.audio.impl.opentk {
  public class AudioOpenTk : IAudio {
    private readonly AudioContext ctx_;
    private readonly AudioFactoryOpenTk factory_ = new AudioFactoryOpenTk();

    public AudioOpenTk() {
      this.ctx_ = new AudioContext();
    }

    public IAudioFactory Factory => this.factory_;

    public void Poll() {
      this.factory_.Poll();
    }
  }

  public class AudioFactoryOpenTk : IAudioFactory {
    // TODO: Use a discardable list.
    private IList<IAudioStreamSource> streamSources_ =
        new List<IAudioStreamSource>();

    public void Poll() {
      foreach (var streamSource in this.streamSources_) {
        if (!streamSource.IsDiscarded) {
          streamSource.PollForProcessedBuffers();
        }
      }
    }

    public IAudioBuffer NewAudioBuffer() => new AudioBufferOpenTk();

    public IAudioSource NewAudioSource() => new AudioSourceOpenTk();

    public IAudioStreamSource NewAudioStreamSource(
        Action<byte[]> populateFunc,
        int channels,
        int bytesPerSample,
        int frequency,
        int numBuffers,
        int bufferSize) {
      var streamSource = new AudioStreamSourceOpenTk(populateFunc,
                                                     channels,
                                                     bytesPerSample,
                                                     frequency,
                                                     numBuffers,
                                                     bufferSize);
      this.streamSources_.Add(streamSource);
      return streamSource;
    }
  }

  public class AudioSourceOpenTk : DiscardableImpl, IAudioSource {
    public int Id { get; }
    private IAudioBuffer? currentBuffer_;

    public AudioSourceOpenTk() {
      this.Id = AL.GenSource();
      this.OnDiscard += _ => this.Destroy_();
    }

    private void Destroy_() {
      AL.DeleteSource(this.Id);
    }

    public void Play(IAudioBuffer buffer, bool loop) =>
        this.Play_((AudioBufferOpenTk) buffer, loop);

    private void Play_(AudioBufferOpenTk buffer, bool loop) {
      if (buffer != this.currentBuffer_ &&
          AL.GetSourceState(this.Id) == ALSourceState.Paused) {
        AL.SourcePlay(this.Id);
        return;
      }

      AL.SourceRewind(this.Id);
      AL.BindBufferToSource(this.Id, buffer.Id);
      this.currentBuffer_ = buffer;

      AL.Source(this.Id, ALSourceb.Looping, loop);

      AL.SourcePlay(this.Id);
    }

    public void Pause() {
      AL.SourcePause(this.Id);
    }

    public void Stop() {
      AL.SourceStop(this.Id);
      this.currentBuffer_ = null;
    }

    public int? SampleOffset {
      get {
        if (this.currentBuffer_ == null) {
          return null;
        }

        AL.GetSource(this.Id, ALGetSourcei.SampleOffset, out var sampleOffset);
        return sampleOffset;
      }
    }

    public float? SampleOffsetFraction =>
        (this.currentBuffer_ != null)
            ? 1f * this.SampleOffset / this.currentBuffer_.SampleCount
            : null;
  }

  public static class PcmHelperOpenTk {
    public static ALFormat GetPcmFormat(int channels, int bytesPerSample) {
      ALFormat? format = channels switch {
          1 => bytesPerSample switch {
              1 => ALFormat.Mono8,
              2 => ALFormat.Mono16,
          },
          2 => bytesPerSample switch {
              1 => ALFormat.Stereo8,
              2 => ALFormat.Stereo16,
          },
      };

      if (format != null) {
        return (ALFormat) format;
      }

      throw new Exception("Unsupported PCM format.");
    }
  }

  public class AudioBufferOpenTk : DiscardableImpl, IAudioBuffer {
    private IPcmData? pcm_;
    public int Id { get; }

    public AudioBufferOpenTk() {
      this.Id = AL.GenBuffer();
      this.OnDiscard += _ => this.Destroy_();
    }

    private void Destroy_() {
      AL.DeleteBuffer(this.Id);
    }

    public int SampleCount => this.pcm_?.SampleCount ?? 0;

    public void FillWithPcm(IPcmData pcm) {
      this.pcm_ = pcm;

      var channels = pcm.Channels;
      var bytesPerSample = pcm.BytesPerSample;
      var format = PcmHelperOpenTk.GetPcmFormat(channels, bytesPerSample);

      var bytes = pcm.Pcm;
      AL.BufferData(this.Id,
                    format,
                    bytes,
                    bytes.Length,
                    pcm.SampleRate);
    }
  }
}