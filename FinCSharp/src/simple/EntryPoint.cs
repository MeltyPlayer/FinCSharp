using fin.pipeline;
using fin.app.impl.opentk;
using System;
using System.Threading.Tasks;

namespace simple {
  public class EntryPoint {
    public static void Main(string[] args) {
      IPipelineStep<int, int> step = Pipeline.Step<int, int>((number) => {
        return number;
      });
      step.Then((number) => {
        return "" + number;
      }).Then((text) => {
        Console.WriteLine(text);
      }, (text) => {
        Console.WriteLine(text);
      });
      step.Call(10).Wait();

      App app = new App();
      app.Launch();
    }
  }
}
