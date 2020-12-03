using fin.discardable;

using OpenTK.Audio.OpenAL;

namespace fin.audio.impl.opentk {
  public partial class AudioOpenTk : IAudio {
    private partial class AudioFactoryOpenTk : IAudioFactory {
      public ISingleAudioSource NewAudioSource()
        => new SingleAudioSourceOpenTk(this.node_);

      private class SingleAudioSourceOpenTk : ISingleAudioSource {
        private readonly IDiscardableNode node_;
        public int Id { get; }
        private IAudioBuffer? currentBuffer_;

        public SingleAudioSourceOpenTk(IDiscardableNode parent) {
          this.Id = AL.GenSource();

          this.node_ = parent.CreateChild();
          this.node_.OnDiscard += _ => this.Destroy_();
        }

        private void Destroy_() => AL.DeleteSource(this.Id);

        public void Play(IAudioBuffer buffer, bool loop, float pitch) =>
            this.Play_((AudioBufferOpenTk) buffer, loop, pitch);

        private void Play_(AudioBufferOpenTk buffer, bool looping, float pitch) {
          if (buffer != this.currentBuffer_ &&
              AL.GetSourceState(this.Id) == ALSourceState.Paused) {
            AL.SourcePlay(this.Id);
            return;
          }

          AL.SourceRewind(this.Id);
          AL.BindBufferToSource(this.Id, buffer.Id);
          this.currentBuffer_ = buffer;

          // TODO: Pull these out as properties of the source?
          AL.Source(this.Id, ALSourceb.Looping, looping);
          AL.Source(this.Id, ALSourcef.Pitch, pitch);

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