using fin.app.impl.opentk;

namespace simple {
  public class EntryPoint {
    public static void Main() {
      using var app = new OpenTkApp();
      app.Launch(null);
    }
  }
}