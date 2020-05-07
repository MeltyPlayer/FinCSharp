using System;
using System.IO;

using fin.app;
using fin.file;

using Newtonsoft.Json;

namespace fin.settings {
  public class Settings {
    public Resolution Resolution { get; } = new Resolution(640, 480);
    public double Framerate { get; } = 60;

    public static Settings Load() {
      const string directoryPath = "data/";
      const string filePath = directoryPath + "settings.json";

      var file = LocalFile.WithinResources(filePath);
      try {
        if (file.Exists()) {
          return JsonUtil.DeserializeFrom<Settings>(file);
        }
      }
      catch (Exception e) {
        // TODO: Handle this exception.
      }

      var settings = new Settings();
      if (!Directory.Exists(directoryPath)) {
        Directory.CreateDirectory(directoryPath);
      }

      var json = JsonConvert.SerializeObject(settings);
      using (var writer = new StreamWriter(filePath)) {
        writer.Write(json);
      }

      return settings;
    }
  }
}