using Newtonsoft.Json;
using RSG;

namespace fin.file {
  public static class JsonUtil {
    public static T DeserializeFrom<T>(IFile file) {
      return JsonConvert.DeserializeObject<T>(FileUtil.ReadText(file));
    }

    public static IPromise<T> DeserializeFromAsync<T>(IFile file) {
      return FileUtil.ReadTextAsync(file)
        .Then((text) => JsonConvert.DeserializeObject<T>(text));
    }

    public static void SerializeTo<T>(IFile file, T instance) {
      var text = JsonConvert.SerializeObject(instance);
      FileUtil.WriteText(file, text);
    }

    /*public static IPromise SerializeToAsync<T>(IFile file, T instance) {
      var text = JsonConvert.SerializeObject(instance);
      return FileUtil.WriteTextAsync(file, text);
    }*/
  }
}