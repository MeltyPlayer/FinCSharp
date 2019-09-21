using fin.app.impl.opentk;

namespace simple {
  public class EntryPoint {
    public static void Main(string[] args) {
      using (var app = new OpenTkApp()) {
        app.Launch(null);
      }
    }
  }
}