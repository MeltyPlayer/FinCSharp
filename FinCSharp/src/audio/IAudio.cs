using System;

namespace fin.audio {
  public interface IAudio {
    IAudioFactory Factory { get; }
  }

  // TODO: Rethink this.
  public interface IAudioFactory {
    IAudioSource NewAudioSource();

    IAudioBuffer NewAudioBuffer();

    IAudioStreamSource NewAudioStreamSource(
        Action<byte[]> populateFunc,
        int frequency = 44100,
        int numBuffers = 2,
        int bufferSize = 4096);
  }

  public interface IAudioSource {
    void Play(IAudioBuffer buffer, bool looping);
    void Pause();
    void Stop();
  }

  public interface IAudioBuffer {
    void FillWithPcm(IPcmData pcm);
  }

  public interface IAudioStreamSource {
    void PollForProcessedBuffers();
    void Play(bool looping);
    void Pause();
    void Stop();
  }

  public interface IPcmData {
    int Channels { get; }
    int BytesPerSample { get; }
    int SampleRate { get; }
    byte[] Pcm { get; }
  }

  public class PcmData : IPcmData {
    public int Channels { get; }
    public int BytesPerSample { get; }
    public int SampleRate { get; }
    public byte[] Pcm { get; }

    public PcmData(
        int channels,
        int bytesPerSample,
        int sampleRate,
        byte[] pcm) {
      this.Channels = channels;
      this.BytesPerSample = bytesPerSample;
      this.SampleRate = sampleRate;
      this.Pcm = pcm;
    }
  }
}