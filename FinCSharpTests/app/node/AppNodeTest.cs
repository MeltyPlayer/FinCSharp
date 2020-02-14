using fin.events;

namespace fin.app.node {

  public class AppNodeTest {
    private static readonly EventType<string> PASS_STRING = new EventType<string>();

    private class PassStringEvent : EventType<string> { }

    private class Foo : AppNode {
    }

    private class Bar : AppNode {

      [OnTick]

      public void PrintToLog(PassString private event, ) {
      }
      }
    }
  }