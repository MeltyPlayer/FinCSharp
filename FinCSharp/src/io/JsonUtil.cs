using Newtonsoft.Json;

using RSG;

namespace fin.io {
  public static class JsonUtil {
    public static T DeserializeFrom<T>(IFile file)
      => JsonConvert.DeserializeObject<T>(FileUtil.ReadText(file));

    public static IPromise<T> DeserializeFromAsync<T>(IFile file)
      => FileUtil.ReadTextAsync(file)
        .Then((text) => JsonConvert.DeserializeObject<T>(text));

    public static void SerializeTo<T>(IFile file, T instance)
      => FileUtil.WriteText(file, JsonConvert.SerializeObject(instance));

    /*public static IPromise SerializeToAsync<T>(IFile file, T instance) {
      var text = JsonConvert.SerializeObject(instance);
      return FileUtil.WriteTextAsync(file, text);
    }*/
  }
}