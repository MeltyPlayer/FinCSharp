using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.app.phase {
  [TestClass]
  public class TickHandlerManagerTest {
    private static string log_;

    private static void ClearLog() {
      TickHandlerManagerTest.log_ = "";
    }

    private static void PrintToLog(string text) {
      TickHandlerManagerTest.log_ += text;
    }

    private static void AssertLog(string log) {
      Assert.AreEqual(TickHandlerManagerTest.log_, log);
    }

    public class ManagerA : IPhaseManager {}

    public class HandlerA : IPhaseHandler<ManagerA> {
      public void OnPhase(ManagerA manager) {
        TickHandlerManagerTest.PrintToLog("A");
      }
    }

    public class ManagerB : IPhaseManager {}

    public class HandlerB : IPhaseHandler<ManagerB> {
      public void OnPhase(ManagerB manager) {
        TickHandlerManagerTest.PrintToLog("B");
      }
    }

    [TestInitialize]
    public void SetUp() {
      TickHandlerManagerTest.ClearLog();
    }

    [TestMethod]
    public void TestTick() {
      var tickHandlerManager = new TickHandlerManager();

      var handlerA = new HandlerA();
      var handlerB = new HandlerB();

      tickHandlerManager.AddHandler(handlerA).AddHandler(handlerB);

      var managerA = new ManagerA();
      var managerB = new ManagerB();

      tickHandlerManager.AddManager(managerA).AddManager(managerB);
      TickHandlerManagerTest.AssertLog("AB");
      TickHandlerManagerTest.ClearLog();

      tickHandlerManager.AddManager(managerB).AddManager(managerA);
      TickHandlerManagerTest.AssertLog("BA");
    }

    [TestMethod]
    public void TestDisposal() {
      Assert.Fail();
    }
  }
}