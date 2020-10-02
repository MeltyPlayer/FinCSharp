using fin.discardable;

using OpenTK.Audio.OpenAL;

namespace fin.audio.impl.opentk {
  public partial class AudioOpenTk : IAudio {
    private partial class AudioFactoryOpenTk : IAudioFactory {
      public IAudioBuffer NewAudioBuffer()
        => new AudioBufferOpenTk(this.node_);

      private class AudioBufferOpenTk : IAudioBuffer {
        private readonly IDiscardableNode node_;
        private IPcmData? pcm_;

        public int Id { get; }

        public AudioBufferOpenTk(IDiscardableNode parent) {
          this.Id = AL.GenBuffer();

          this.node_ = parent.CreateChild();
          this.node_.OnDiscard += _ => this.Destroy_();
        }

        private void Destroy_() => AL.DeleteBuffer(this.Id);

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
  }
}