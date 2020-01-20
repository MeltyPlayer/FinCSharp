using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.app.phase {

  [TestClass]
  public partial class TickHandlerTest {
    private static TickHandler TICK_HANDLER = new TickHandler();
    private static ManagerA MANAGER_A = new ManagerA();
    private static ManagerB MANAGER_B = new ManagerB();
    private static ManagerC MANAGER_C = new ManagerC();
    private static ManagerD MANAGER_D = new ManagerD();
    private static ManagerE MANAGER_E = new ManagerE();
    private static ManagerFGH MANAGER_FGH = new ManagerFGH();

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