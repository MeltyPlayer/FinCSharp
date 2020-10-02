using System;
using System.Collections.Generic;

using fin.discardable;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace fin.audio.impl.opentk {
  // TODO: Discardables need to be passed from owner, NOT this.
  public partial class AudioOpenTk : IAudio {
    private readonly IDiscardableNode node_;
    private readonly AudioFactoryOpenTk factory_;

    public AudioOpenTk(IDiscardableNode parent) {
      this.node_ = parent.CreateChild();
      this.node_.Using(new AudioContext());

      this.factory_ = new AudioFactoryOpenTk(this.node_);
    }

    public IAudioFactory Factory => this.factory_;

    public void Poll() => this.factory_.Poll();

    private partial class AudioFactoryOpenTk : IAudioFactory {
      private readonly IDiscardableNode node_;

      // TODO: Use a discardable list.
      private readonly IList<IAudioStreamSource> streamSources_ =
          new List<IAudioStreamSource>();

      public AudioFactoryOpenTk(IDiscardableNode parent) {
        this.node_ = parent.CreateChild();
      }

      public void Poll() {
        foreach (var streamSource in this.streamSources_) {
          //if (!streamSource.IsDiscarded) {
            streamSource.PollForProcessedBuffers();
          //}
        }
      }
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
  }
}