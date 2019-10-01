using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.app.phase {

  [TestClass]
  public class TickHandlerTest {
    private static TickHandler TICK_HANDLER = new TickHandler();
    private static ManagerA MANAGER_A = new ManagerA();
    private static ManagerB MANAGER_B = new ManagerB();
    private static ManagerC MANAGER_C = new ManagerC();
    private static ManagerD MANAGER_D = new ManagerD();
    private static ManagerE MANAGER_E = new ManagerE();
    private static ManagerFGH MANAGER_FGH = new ManagerFGH();
    private static string LOG = "";

    public static void ClearLog() {
      LOG = "";
    }

    public static void PrintToLog(string text) {
      LOG += text + "_";
    }

    public static void AssertLog(string log) => Assert.AreEqual(log, LOG);

    public class ManagerA { }

    public class HandlerA : ReflectivePhaseHandler, IReflectivePhaseHandler<ManagerA> {

      public void OnPhase(ManagerA manager) => PrintToLog("A");
    }

    public class ManagerB { }

    public class HandlerB : ReflectivePhaseHandler, IReflectivePhaseHandler<ManagerB> {

      public void OnPhase(ManagerB manager) => PrintToLog("B");
    }

    public abstract class ManagerCBase { }

    public class ManagerC : ManagerCBase { }

    public class HandlerC : ReflectivePhaseHandler, IReflectivePhaseHandler<ManagerCBase> {

      public void OnPhase(ManagerCBase manager) => PrintToLog("C");
    }

    public interface IManagerDBase { }

    public class ManagerD : IManagerDBase { }

    public class HandlerD : ReflectivePhaseHandler, IReflectivePhaseHandler<IManagerDBase> {

      public void OnPhase(IManagerDBase manager) => PrintToLog("D");
    }

    public class ManagerE { }

    public interface IHandlerE : IReflectivePhaseHandler<ManagerE> { }

    public class HandlerE : ReflectivePhaseHandler, IHandlerE {

      public void OnPhase(ManagerE manager) => PrintToLog("E");
    }

    public abstract class ManagerF { }

    public interface IManagerG { }

    public class ManagerFGH : ManagerF, IManagerG { }

    public class HandlerF : ReflectivePhaseHandler, IReflectivePhaseHandler<ManagerF> {

      public void OnPhase(ManagerF manager) => PrintToLog("F");
    }

    public class HandlerG : ReflectivePhaseHandler, IReflectivePhaseHandler<IManagerG> {

      public void OnPhase(IManagerG manager) => PrintToLog("G");
    }

    public class HandlerFGH : ReflectivePhaseHandler, IReflectivePhaseHandler<ManagerFGH> {

      public void OnPhase(ManagerFGH manager) => PrintToLog("FGH");
    }

    public class HandlerAB : ReflectivePhaseHandler, IReflectivePhaseHandler<ManagerA>, IReflectivePhaseHandler<ManagerB> {

      public void OnPhase(ManagerA manager) => PrintToLog("A*");

      public void OnPhase(ManagerB manager) => PrintToLog("B*");
    }

    [TestInitialize]
    public void SetUp() {
      ClearLog();

      TICK_HANDLER = new TickHandler();

      MANAGER_A = new ManagerA();
      MANAGER_B = new ManagerB();
      MANAGER_C = new ManagerC();
      MANAGER_D = new ManagerD();
      MANAGER_E = new ManagerE();
      MANAGER_FGH = new ManagerFGH();

      TICK_HANDLER.AddHandlers(
        new HandlerA(),
        new HandlerB(),
        new HandlerC(),
        new HandlerD(),
        new HandlerE(),
        new HandlerF(),
        new HandlerG(),
        new HandlerFGH(),
        new HandlerAB()
      );
    }

    [TestMethod]
    public void TestTickA() {
      TICK_HANDLER.Tick(new object[] { MANAGER_A });
      AssertLog("A_A*_");
    }

    [TestMethod]
    public void TestTickAB() {
      TICK_HANDLER.Tick(new object[] { MANAGER_A, MANAGER_B });
      AssertLog("A_A*_B_B*_");
    }

    [TestMethod]
    public void TestTickBA() {
      TICK_HANDLER.Tick(new object[] { MANAGER_B, MANAGER_A });
      AssertLog("B_B*_A_A*_");
    }

    [TestMethod]
    public void TestTickABA() {
      TICK_HANDLER.Tick(new object[] { MANAGER_A, MANAGER_B, MANAGER_A });
      AssertLog("A_A*_B_B*_A_A*_");
    }

    [TestMethod]
    public void TestTickC() {
      TICK_HANDLER.Tick(new object[] { MANAGER_C });
      AssertLog("C_");
    }

    [TestMethod]
    public void TestTickD() {
      TICK_HANDLER.Tick(new object[] { MANAGER_D });
      AssertLog("D_");
    }

    [TestMethod]
    public void TestTickE() {
      TICK_HANDLER.Tick(new object[] { MANAGER_E });
      AssertLog("E_");
    }

    [TestMethod]
    public void TestTickFGH() {
      TICK_HANDLER.Tick(new object[] { MANAGER_FGH });
      AssertLog("F_G_FGH_");
    }

    [TestMethod]
    public void TestDisposal() {
      Assert.Fail();
    }
  }
}