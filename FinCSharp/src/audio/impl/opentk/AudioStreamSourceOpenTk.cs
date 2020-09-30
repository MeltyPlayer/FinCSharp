using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using fin.discardable;
using fin.math.number;

using OpenTK.Audio.OpenAL;

namespace fin.audio.impl.opentk {
  public enum AudioStreamSourceState {
    STOPPED,
    PLAYING,
    PAUSED,

    DESTROYED,
  }

  // TODO: Rewrite this to stream an observable.
  // TODO: Handle non-looping?
  public class AudioStreamSourceOpenTk : DiscardableImpl, IAudioStreamSource {
    private readonly int sourceId_;

    private readonly ImmutableArray<int> bufferIds_;
    private readonly Action<byte[]> populateFunc_;
    private readonly ALFormat format_;
    private readonly int frequency_;
    private readonly int bufferSize_;
    private readonly CircularRangedNumber<int> currentBufferIndex_;

    private readonly Queue<int> readyBuffersIds_;

    // TODO: Switch to AL states instead.
    private AudioStreamSourceState state_ =
        AudioStreamSourceState.STOPPED;

    public AudioStreamSourceOpenTk(
        Action<byte[]> populateFunc,
        int channels,
        int bytesPerSample,
        int frequency,
        int numBuffers,
        int bufferSize) {
      this.sourceId_ = AL.GenSource();

      this.bufferIds_ = AL.GenBuffers(numBuffers).ToImmutableArray();
      this.populateFunc_ = populateFunc;

      this.format_ = PcmHelperOpenTk.GetPcmFormat(channels, bytesPerSample);

      this.frequency_ = frequency;
      this.bufferSize_ = bufferSize;

      this.currentBufferIndex_ =
          new CircularRangedNumber<int>(0, 0, numBuffers);

      // TODO: Delay this until the observable has returned some value. Stream
      // should remember stop/play/paused state as expected in the meantime.
      this.readyBuffersIds_ = new Queue<int>();
      foreach (var bufferId in this.bufferIds_) {
        this.readyBuffersIds_.Enqueue(bufferId);
      }
      this.PopulateAndQueueReadyBuffers_();

      this.OnDiscard += _ => this.Destroy_();
    }

    public void PollForProcessedBuffers() {
      AL.GetSource(this.sourceId_,
                   ALGetSourcei.BuffersProcessed,
                   out var processed);

      for (var i = 0; i < processed; ++i) {
        var processedBufferId =
            this.bufferIds_[this.currentBufferIndex_.Value++];

        AL.SourceUnqueueBuffers(this.sourceId_, 1);
        this.readyBuffersIds_.Enqueue(processedBufferId);
      }

      this.PopulateAndQueueReadyBuffers_();
    }

    private void PopulateAndQueueReadyBuffers_() {
      while (this.readyBuffersIds_.Count > 0) {
        var readyBufferId = this.readyBuffersIds_.Dequeue();
        this.PopulateAndQueueBuffer_(readyBufferId);
      }
    }

    private void PopulateAndQueueBuffer_(int bufferId) {
      byte[] pcm = new byte[this.bufferSize_];
      this.populateFunc_(pcm);

      AL.BufferData(bufferId,
                    this.format_,
                    pcm,
                    this.bufferSize_,
                    this.frequency_);
      AL.SourceQueueBuffer(this.sourceId_, bufferId);
    }

    public void Play(bool loop) {
      switch (this.state_) {
        case AudioStreamSourceState.PAUSED:
        case AudioStreamSourceState.STOPPED:
          this.state_ = AudioStreamSourceState.PLAYING;
          AL.SourcePlay(this.sourceId_);
          break;
      }
    }

    public void Pause() {
      if (this.state_ == AudioStreamSourceState.PLAYING) {
        this.state_ = AudioStreamSourceState.PAUSED;
        AL.SourcePause(this.sourceId_);
      }
    }

    public void Stop() {
      switch (this.state_) {
        case AudioStreamSourceState.PLAYING:
        case AudioStreamSourceState.PAUSED:
          this.state_ = AudioStreamSourceState.STOPPED;
          AL.SourceStop(this.sourceId_);
          AL.SourceRewind(this.sourceId_);
          AL.SourceUnqueueBuffer(this.sourceId_);
          break;
      }
    }

    private void Destroy_() {
      this.state_ = AudioStreamSourceState.DESTROYED;
      AL.DeleteSource(this.sourceId_);
      AL.DeleteBuffers(this.bufferIds_.ToArray());
      this.bufferIds_.Clear();
    }
  }
}