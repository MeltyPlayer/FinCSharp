using System;

using fin.file;
using fin.log;

using NVorbis;

namespace fin.audio {
  public interface IOggLoader {
    IPcmData Load(IFile file);
  }

  public class OggLoader : IOggLoader {
    public IPcmData Load(IFile file) {
      var ogg = new VorbisReader(file.uri);

      var sampleCount = (int) ogg.TotalSamples;

      var floatPcm = new float[sampleCount];
      ogg.ReadSamples(floatPcm, 0, sampleCount);

      //var floatMin = -32768;
      //var floatMax = 32767;

      var floatMin = -1;
      var floatMax = 1;

      float byteMin = Byte.MinValue;
      float byteMax = Byte.MaxValue;

      var bytePcm = new byte[sampleCount];
      for (var i = 0; i < sampleCount; ++i) {
        var floatSample = floatPcm[i];
        var normalizedFloatSample =
            (MathF.Max(floatMin, Math.Min(floatSample, floatMax)) - floatMin) /
            (floatMax - floatMin);

        bytePcm[i] = (byte) Math.Round(byteMin + normalizedFloatSample * (byteMax - byteMin));
      }

      return new PcmData(
          ogg.Channels,
          1,
          ogg.SampleRate,
          bytePcm);
    }
  }
}