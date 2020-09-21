using System;
using System.Collections.Immutable;
using System.Linq;

using fin.discardable;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace fin.audio.impl.opentk {
  public class AudioOpenTk : IAudio {
    private readonly AudioContext ctx_;

    public AudioOpenTk() {
      this.ctx_ = new AudioContext();
    }

    public IAudioFactory Factory { get; } = new AudioFactoryOpenTk();
  }

  public class AudioFactoryOpenTk : IAudioFactory {
    public IAudioBuffer NewAudioBuffer() => new AudioBufferOpenTk();

    public IAudioSource NewAudioSource() => new AudioSourceOpenTk();

    public IAudioStreamSource NewAudioStreamSource(
        Action<byte[]> populateFunc,
        int frequency = 44100,
        int numBuffers = 2,
        int bufferSize = 4096) =>
        new AudioStreamSourceOpenTk(populateFunc,
                                    frequency,
                                    numBuffers,
                                    bufferSize);
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

      ALFormat? format = null;

      var channels = pcm.Channels;
      var bytesPerSample = pcm.BytesPerSample;
      format = channels switch {
          1 => bytesPerSample switch {
              1 => ALFormat.Mono8,
              2 => ALFormat.Mono16,
          },
          2 => bytesPerSample switch {
              1 => ALFormat.Stereo8,
              2 => ALFormat.Stereo16,
          },
      };

      var bytes = pcm.Pcm;

      if (format != null) {
        AL.BufferData(this.Id,
                      (ALFormat) format,
                      bytes,
                      bytes.Length,
                      pcm.SampleRate);
      }
      else {
        throw new FormatException("Unsupported audio format.");
      }
    }
  }

  public enum AudioStreamSourceState {
    READY_TO_PLAY,
    PLAYING,
    PAUSED,
    STOPPED,
  }

  public class AudioStreamSourceOpenTk : DiscardableImpl, IAudioStreamSource {
    private readonly int sourceId_;

    private readonly ImmutableArray<int> bufferIds_;
    private readonly Action<byte[]> populateFunc_;
    private readonly int frequency_;
    private readonly int bufferSize_;
    private int expectedCurrentBufferIndex_ = 0;

    private readonly bool[] readyToProcess_;

    private AudioStreamSourceState state_ = AudioStreamSourceState.STOPPED;

    public AudioStreamSourceOpenTk(
        Action<byte[]> populateFunc,
        int frequency = 44100,
        int numBuffers = 2,
        int bufferSize = 4096) {
      this.sourceId_ = AL.GenSource();

      this.bufferIds_ = AL.GenBuffers(numBuffers).ToImmutableArray();
      this.populateFunc_ = populateFunc;
      this.frequency_ = frequency;
      this.bufferSize_ = bufferSize;

      this.readyToProcess_ = new bool[numBuffers];
      this.readyToProcess_[0] = true;

      this.OnDiscard += _ => this.Destroy_();
    }

    //TODO: Don't play until stream is ready.

    public void PollForProcessedBuffers() {
      bool bufferProcessed;

      do {
        var expectedBufferId =
            this.bufferIds_[this.expectedCurrentBufferIndex_];
        AL.GetSource(this.sourceId_,
                     ALGetSourcei.Buffer,
                     out var actualBufferId);

        bufferProcessed = expectedBufferId != actualBufferId;
        if (bufferProcessed) {
          byte[] pcm = new byte[this.bufferSize_];
          this.populateFunc_(pcm);

          AL.BufferData(expectedBufferId,
                        ALFormat.Mono8,
                        pcm,
                        this.bufferSize_,
                        this.frequency_);

          ++this.expectedCurrentBufferIndex_;
          if (this.expectedCurrentBufferIndex_ == this.bufferIds_.Length) {
            this.expectedCurrentBufferIndex_ = 0;
          }
        }
      } while (bufferProcessed);
    }

    public void Play(bool loop) {
      if (AL.GetSourceState(this.sourceId_) == ALSourceState.Paused) {
        AL.SourcePlay(this.sourceId_);
        return;
      }

      AL.SourceRewind(this.sourceId_);
      AL.SourceQueueBuffers(this.sourceId_,
                            this.bufferIds_.Length,
                            this.bufferIds_.ToArray());

      AL.Source(this.sourceId_, ALSourceb.Looping, loop);

      AL.SourcePlay(this.sourceId_);
    }

    public void Pause() {
      AL.SourcePause(this.sourceId_);
    }

    public void Stop() {
      AL.SourceStop(this.sourceId_);
    }

    private void Destroy_() {
      AL.DeleteSource(this.sourceId_);
      AL.DeleteBuffers(this.bufferIds_.ToArray());
      this.bufferIds_.Clear();
    }
  }
}