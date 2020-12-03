using System;

using fin.io;
using fin.log;

using NVorbis;

namespace fin.audio {
  public interface IOggLoader {
    IPcmData Load(IFile file);
  }

  public class OggLoader : IOggLoader {
    public IPcmData Load(IFile file) {
      using var ogg = new VorbisReader(file.Uri);

      var channels = ogg.Channels;
      var sampleCount = (int) ogg.TotalSamples * channels;

      var floatPcm = new float[sampleCount];
      ogg.ReadSamples(floatPcm, 0, sampleCount);

      const int bytesPerSample = 2;

      //var floatMin = -32768;
      //var floatMax = 32767;

      var floatMin = -1f;
      var floatMax = 1f;

      float byteMin = Byte.MinValue;
      float byteMax = Byte.MaxValue;

      var bytePcm = new byte[sampleCount * bytesPerSample];
      for (var i = 0; i < sampleCount; ++i) {
        var floatSample = floatPcm[i];
        OggLoader.StoreFloatAsBytes_(floatSample,
                                     bytePcm,
                                     i * bytesPerSample,
                                     bytesPerSample);
      }

      return new PcmData(
          channels,
          bytesPerSample,
          ogg.SampleRate,
          bytePcm);
    }

    private static void StoreFloatAsBytes_(
        float value,
        byte[] bytes,
        int offset,
        int bytesPerSample) {
      /*var normalizedFloatSample =
          (MathF.Max(floatMin, Math.Min(floatSample, floatMax)) - floatMin) /
          (floatMax - floatMin);
      
               bytePcm[i] =
            (byte) Math.Round(byteMin +
                              normalizedFloatSample * (byteMax - byteMin));
       */

      if (bytesPerSample == 2) {
        var floatMin = -1f;
        var floatMax = 1f;

        var normalizedFloatSample =
            (MathF.Max(floatMin, Math.Min(value, floatMax)) - floatMin) /
            (floatMax - floatMin);

        float shortMin = short.MinValue;
        float shortMax = short.MaxValue;

        var shortSample =
            (short) Math.Round(shortMin +
                               normalizedFloatSample * (shortMax - shortMin));

        for (var i = 0; i < 2; ++i) {
          bytes[offset + i] = (byte) (shortSample >> (8 * i));
        }
      }
    }
  }
}