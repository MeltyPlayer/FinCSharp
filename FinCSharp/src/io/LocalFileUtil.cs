using System;
using System.IO;
using System.Threading.Tasks;

using RSG;

namespace fin.io {
  public static class LocalFileUtil {
    public static byte[] ReadBytes(LocalFile file) =>
        File.ReadAllBytes(file.Uri);

    public static string ReadText(LocalFile file) {
      using var reader = new StreamReader(file.Uri);
      return reader.ReadToEnd();
    }

    public static IPromise<string> ReadTextAsync(LocalFile file) {
      var promise = new Promise<string>();

      Task.Run(async () => {
        try {
          using var reader = new StreamReader(file.Uri);
          var text = await reader.ReadToEndAsync();
          promise.Resolve(text);
        }
        catch (Exception e) {
          promise.Reject(e);
        }
      });

      return promise;
    }

    public static IPromise WriteTextAsync(LocalFile file, string text) {
      var promise = new Promise();

      Task.Run(async () => {
        try {
          using var writer = new StreamWriter(file.Uri);
          await writer.WriteAsync(text);
          promise.Resolve();
        }
        catch (Exception e) {
          promise.Reject(e);
        }
      });

      return promise;
    }
  }
}