using System;
using System.IO;
using System.Threading.Tasks;

using RSG;

namespace fin.file {
  internal static class LocalFileUtil {
    static LocalFileUtil() {
      FileUtil.readHandlers.DefineHandler<LocalFile>(LocalFileUtil.ReadText);
      FileUtil.asyncReadHandlers.DefineHandler<LocalFile>(LocalFileUtil
                                                              .ReadTextAsync);
      //FileUtil.writeHandlers.DefineHandler<LocalFile>(WriteText);
    }

    public static byte[] ReadBytes(LocalFile file) =>
        File.ReadAllBytes(file.uri);

    public static string ReadText(LocalFile file) {
      using (var reader = new StreamReader(file.uri)) {
        return reader.ReadToEnd();
      }
    }

    public static IPromise<string> ReadTextAsync(LocalFile file) {
      var promise = new Promise<string>();

      Task.Run(async () => {
        try {
          using (var reader = new StreamReader(file.uri)) {
            var text = await reader.ReadToEndAsync();
            promise.Resolve(text);
          }
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
          using (var writer = new StreamWriter(file.uri)) {
            await writer.WriteAsync(text);
            promise.Resolve();
          }
        }
        catch (Exception e) {
          promise.Reject(e);
        }
      });

      return promise;
    }
  }
}