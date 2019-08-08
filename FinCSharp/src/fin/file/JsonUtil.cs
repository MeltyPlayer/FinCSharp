using Newtonsoft.Json;
using RSG;

namespace fin.file {
  public static class JsonUtil {
    public static IPromise<T> DeserializeFromFile<T>(string filePath) {
      return
        FileUtil.ReadText(filePath)
          .Then((text) => JsonConvert.DeserializeObject<T>(text));
    }

    public static IPromise SerializeToFile<T>(string filePath, T instance) {
      var text = JsonConvert.SerializeObject(instance);
      return FileUtil.WriteText(filePath, text);
    }
  }
}