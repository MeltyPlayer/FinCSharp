using System;
using System.Reactive.Subjects;

using fin.discardable;

namespace fin.audio {
  public interface IAudio {
    IAudioFactory Factory { get; }

    void Poll();
  }

  // TODO: Rethink this.
  public interface IAudioFactory {
    private const int DEFAULT_FREQUENCY = 44100;

    private const int DEFAULT_BUFFER_SIZE =
        2 * 2 * IAudioFactory.DEFAULT_FREQUENCY;

    IAudioSource NewAudioSource();

    IAudioBuffer NewAudioBuffer();

    IAudioStreamSource NewAudioStreamSource(
        Action<byte[]> populateFunc,
        int channels = 1,
        int bytesPerSample = 1,
        int frequency = IAudioFactory.DEFAULT_FREQUENCY,
        int numBuffers = 2,
        int bufferSize = IAudioFactory.DEFAULT_BUFFER_SIZE);
    IAudioStreamSource NewAudioStreamSource(
        Subject<byte[]> populateSubject,
        int channels = 1,
        int bytesPerSample = 1,
        int frequency = IAudioFactory.DEFAULT_FREQUENCY,
        int numBuffers = 2,
        int bufferSize = IAudioFactory.DEFAULT_BUFFER_SIZE);
  }

  public interface IAudioSource {
    void Play(IAudioBuffer buffer, bool looping);
    void Pause();
    void Stop();
    int? SampleOffset { get; }
    float? SampleOffsetFraction { get; }
  }

  public interface IAudioBuffer {
    int SampleCount { get; }
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
    int SampleCount { get; }

    float GetMonoAmplitude(int offset);
    float GetLeftAmplitude(int offset);
    float GetRightAmplitude(int offset);
  }

  public class PcmData : IPcmData {
    public int Channels { get; }
    public int BytesPerSample { get; }
    public int SampleRate { get; }
    public byte[] Pcm { get; }
    public int SampleCount { get; }

    public PcmData(
        int channels,
        int bytesPerSample,
        int sampleRate,
        byte[] pcm) {
      this.Channels = channels;
      this.BytesPerSample = bytesPerSample;
      this.SampleRate = sampleRate;
      this.Pcm = pcm;

      this.SampleCount = this.Pcm.Length / this.Channels / this.BytesPerSample;
    }

    public float GetMonoAmplitude(int offset) =>
        (this.Channels == 1)
            ? this.GetLeftAmplitude(offset)
            : (this.GetLeftAmplitude(offset) + this.GetRightAmplitude(offset)) /
              2;

    public float GetLeftAmplitude(int offset) =>
        this.GetAmplitudeAtOffset_(offset);

    public float GetRightAmplitude(int offset) =>
        (this.Channels == 1)
            ? this.GetLeftAmplitude(offset)
            : this.GetAmplitudeAtOffset_(offset, 1);

    private float GetAmplitudeAtOffset_(
        int sampleOffset,
        int channelOffset = 0) {
      if (this.BytesPerSample == 1) {
        // TODO: Get amplitude for bytes
        return 0;
      }
      else {
        var byteOffset = this.BytesPerSample *
                         (sampleOffset * this.Channels + channelOffset);

        var shortSample =
            (short) ((this.Pcm[byteOffset + 1] << 8) | this.Pcm[byteOffset]);

        var zeroToOneNormalizedSample = 1f * (shortSample - short.MinValue) /
                                        (short.MaxValue - short.MinValue);
        var negativeOneToOneNormalizedSample =
            -1 + 2 * zeroToOneNormalizedSample;

        return negativeOneToOneNormalizedSample;
      }
    }
  }
}