using fin.program;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.settings {
  public class Settings {
    public readonly Resolution resolution = new Resolution(640, 480);
    public readonly double framerate = 60;

    public static Settings Load() {
      string directoryPath = "data/";
      string filePath = directoryPath + "settings.json";

      try {
        if (File.Exists(filePath)) {
          using (StreamReader reader = new StreamReader(filePath)) {
            return JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd());
          }
        }
      } catch (Exception e) {
        // TODO: Handle this exception.
      }

      Settings settings = new Settings();
      if (!Directory.Exists(directoryPath)) {
        Directory.CreateDirectory(directoryPath);
      }

      string json = JsonConvert.SerializeObject(settings);
      using (StreamWriter writer = new StreamWriter(filePath)) {
        writer.Write(json);
      }

      return settings;
    }
  }
}
