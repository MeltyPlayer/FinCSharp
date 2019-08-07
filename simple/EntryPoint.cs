using fin.pipeline;
using fin.program.impl.opentk;

namespace simple {
  public class EntryPoint {
    public static void Main(string[] args) {
      Pipeline.Step((number) => {
        return number;
      }).Then((number) => {
        return "" + number;
      }).Then((text) => {
        Console.WriteLn(text);
      }, (text) => {
        Console.WriteLn(text);
      }).Call(10);

      App app = new App();
      app.Launch();
    }
  }
}
