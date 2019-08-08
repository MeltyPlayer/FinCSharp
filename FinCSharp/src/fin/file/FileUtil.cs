using RSG;
using System;
using System.IO;
using System.Threading.Tasks;

namespace fin.file {
  public static class FileUtil {
    public static IPromise<string> ReadText(string filePath) {
      var promise = new Promise<string>();

      Task.Run(async () => {
        try {
          using (var reader = new StreamReader(filePath)) {
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

    public static IPromise WriteText(string filePath, string text) {
      var promise = new Promise();

      Task.Run(async () => {
        try {
          using (var writer = new StreamWriter(filePath)) {
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