using fin.discardable;

using OpenTK.Audio.OpenAL;

namespace fin.audio.impl.opentk {
  public partial class AudioOpenTk : IAudio {
    private partial class AudioFactoryOpenTk : IAudioFactory {
      public IAudioSource NewAudioSource()
        => new AudioSourceOpenTk(this.node_);

      private class AudioSourceOpenTk : IAudioSource {
        private readonly IDiscardableNode node_;
        public int Id { get; }
        private IAudioBuffer? currentBuffer_;

        public AudioSourceOpenTk(IDiscardableNode parent) {
          this.Id = AL.GenSource();

          this.node_ = parent.CreateChild();
          this.node_.OnDiscard += _ => this.Destroy_();
        }

        private void Destroy_() => AL.DeleteSource(this.Id);

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

            AL.GetSource(this.Id,
                         ALGetSourcei.SampleOffset,
                         out var sampleOffset);
            return sampleOffset;
          }
        }

        public float? SampleOffsetFraction =>
            (this.currentBuffer_ != null)
                ? 1f * this.SampleOffset / this.currentBuffer_.SampleCount
                : null;
      }
    }
  }
}