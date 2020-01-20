using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.app.phase {

  public partial class TickHandlerTest {
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
  }
}