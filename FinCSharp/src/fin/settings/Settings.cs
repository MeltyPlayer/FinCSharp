using fin.app;
using fin.file;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace fin.settings {
  public class Settings {
    public readonly Resolution resolution = new Resolution(640, 480);
    public readonly double framerate = 60;

    public static Settings Load() {
      const string directoryPath = "data/";
      const string filePath = directoryPath + "settings.json";

      try {
        if (File.Exists(filePath)) {
          using (StreamReader reader = new StreamReader(filePath)) {
            return JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd());
          }
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