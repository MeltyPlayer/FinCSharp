using fin.data.collections.buffer;

namespace fin.audio.impl.opentk {
  public partial class AudioOpenTk : IAudio {
    private partial class AudioFactoryOpenTk : IAudioFactory {
      public IPolyphonicAudioSource NewAudioSource(int voiceCount)
        => new PolyphonicAudioSourceOpenTk(this, voiceCount);

      private class PolyphonicAudioSourceOpenTk : IPolyphonicAudioSource {
        private readonly IFinBuffer<IAudioSource> sources_;

        public PolyphonicAudioSourceOpenTk(
            IAudioFactory factory,
            int voiceCount) {
          this.sources_ =
              new FinCircularBuffer<IAudioSource>(
                  voiceCount,
                  factory.NewAudioSource);
        }

        public void Play(IAudioBuffer buffer, bool looping, float pitch)
          => this.sources_.Next.Play(buffer, looping, pitch);

        public void Pause() {
          foreach (var source in this.sources_) {
            source.Pause();
          }
        }

        public void Stop() {
          foreach (var source in this.sources_) {
            source.Stop();
          }
        }

        public int VoiceCount => this.sources_.Count;
      }
    }
  }
}