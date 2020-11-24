using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;

using fin.discardable;
using fin.math.number;

using OpenTK.Audio.OpenAL;

namespace fin.audio.impl.opentk {
  public partial class AudioOpenTk : IAudio {
    private partial class AudioFactoryOpenTk : IAudioFactory {
      public IAudioStreamSource NewAudioStreamSource(
          Subject<byte[]> populateFunc,
          int channels,
          int bytesPerSample,
          int frequency,
          int numBuffers,
          int bufferSize) {
        var streamSource = new ReactiveAudioStreamSourceOpenTk(this.node_,
                                                               populateFunc,
                                                               channels,
                                                               bytesPerSample,
                                                               frequency,
                                                               numBuffers,
                                                               bufferSize);
        this.streamSources_.Add(streamSource);
        return streamSource;
      }

      // TODO: Rewrite this to stream an observable.
      // TODO: Handle non-looping?
      private class ReactiveAudioStreamSourceOpenTk : IAudioStreamSource {
        private readonly IDiscardableNode node_;

        private readonly int sourceId_;

        private readonly ImmutableArray<int> bufferIds_;
        private readonly ALFormat format_;
        private readonly int frequency_;
        private readonly int bufferSize_;
        private readonly CircularRangedInt currentBufferIndex_;

        private readonly Queue<int> readyBuffersIds_;

        // TODO: Switch to AL states instead.
        private AudioStreamSourceState state_ =
            AudioStreamSourceState.STOPPED;

        public ReactiveAudioStreamSourceOpenTk(
            IDiscardableNode parent,
            Subject<byte[]> populateSubject,
            int channels,
            int bytesPerSample,
            int frequency,
            int numBuffers,
            int bufferSize) {
          this.node_ = parent.CreateChild();
          this.node_.OnDiscard += _ => this.Destroy_();

          this.sourceId_ = AL.GenSource();

          this.bufferIds_ = AL.GenBuffers(numBuffers).ToImmutableArray();
          populateSubject.Subscribe(pcm => {
            var readyBufferId = this.readyBuffersIds_.Dequeue();
            this.PopulateAndQueueBuffer_(readyBufferId, pcm);
          });

          this.format_ = PcmHelperOpenTk.GetPcmFormat(channels, bytesPerSample);

          this.frequency_ = frequency;
          this.bufferSize_ = bufferSize;

          this.currentBufferIndex_ = new CircularRangedInt(0, 0, numBuffers);

          // TODO: Delay this until the observable has returned some value. Stream
          // should remember stop/play/paused state as expected in the meantime.
          this.readyBuffersIds_ = new Queue<int>();
          foreach (var bufferId in this.bufferIds_) {
            this.readyBuffersIds_.Enqueue(bufferId);
          }
        }

        private void Destroy_() {
          this.state_ = AudioStreamSourceState.DESTROYED;
          AL.DeleteSource(this.sourceId_);
          AL.DeleteBuffers(this.bufferIds_.ToArray());
          this.bufferIds_.Clear();
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
        }

        private void PopulateAndQueueBuffer_(int bufferId, byte[] pcm) {
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
      }
    }
  }
}