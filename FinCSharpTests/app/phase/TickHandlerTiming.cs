using fin.function;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace fin.app.phase {
  public partial class TickHandlerTest {
    public static TimeSpan TimeMethod(int times, Action handler) {
      var initialTime = DateTime.UtcNow;
      for (int i = 0; i < times; ++i) {
        handler();
      }
      var finalTime = DateTime.UtcNow;

      return (finalTime - initialTime) / times;
    }

    [TestMethod]
    public void TestTimeSpan() {
      var expectedLog = "A_A*_B_B*_C_D_E_F_G_FGH_";
      Action handleViaTickHandler = () =>
        TICK_HANDLER.Tick(new object[] { MANAGER_A, MANAGER_B, MANAGER_C, MANAGER_D, MANAGER_E, MANAGER_FGH });

      var handlerA = new HandlerA();
      var handlerB = new HandlerB();
      var handlerC = new HandlerC();
      var handlerD = new HandlerD();
      var handlerE = new HandlerE();
      var handlerF = new HandlerF();
      var handlerG = new HandlerG();
      var handlerFgh = new HandlerFGH();
      var handlerAb = new HandlerAB();
      Action handleNaively = () => {
        handlerA.OnPhase(MANAGER_A);
        handlerAb.OnPhase(MANAGER_A);

        handlerB.OnPhase(MANAGER_B);
        handlerAb.OnPhase(MANAGER_B);

        handlerC.OnPhase(MANAGER_C);

        handlerD.OnPhase(MANAGER_D);

        handlerE.OnPhase(MANAGER_E);

        handlerF.OnPhase(MANAGER_FGH);
        handlerG.OnPhase(MANAGER_FGH);
        handlerFgh.OnPhase(MANAGER_FGH);
      };

      Action<Action> callThenClear = (Action handler) => {
        handler();
        ClearLog();
      };

      /*var dataTypesToHandlerCache = ReflectivePhaseManager.ReflectivelyAcquireDataTypesToHandlerDictionaryImpl_.Cache;
      var allTypesCache = ReflectivePhaseManager.ReflectivelyAcquireAllTypesImpl_.Cache;
      var compatibleDataTypesCache = ReflectivePhaseManager.ReflectivelyAcquireCompatibleDataTypesImpl_.Cache;

      Assert.AreEqual(9, dataTypesToHandlerCache.Count);
      Assert.AreEqual(0, allTypesCache.Count);
      Assert.AreEqual(0, compatibleDataTypesCache.Count);

      callThenClear(() => {
        handleViaTickHandler();
        AssertLog(expectedLog);
      });
      callThenClear(() => {
        handleNaively();
        AssertLog(expectedLog);
      });

      var expectedDataTypesToHandlerCacheCount = 9;
      var expectedAllTypesCacheCount = 6;
      var expectedCompatibleDataTypesCacheCount = 16;
      Assert.AreEqual(expectedDataTypesToHandlerCacheCount, dataTypesToHandlerCache.Count);
      Assert.AreEqual(expectedAllTypesCacheCount, allTypesCache.Count);
      Assert.AreEqual(expectedCompatibleDataTypesCacheCount, compatibleDataTypesCache.Count); */

      var times = 10000;
      var tickHandlerTimeSpan = TimeMethod(times, () => callThenClear(handleViaTickHandler));
      var tickHandlerLogLength = LOG.Length;
      var naiveTimeSpan = TimeMethod(times, () => callThenClear(handleNaively));
      var naiveLogLength = LOG.Length;

      /* Assert.AreEqual(expectedDataTypesToHandlerCacheCount, dataTypesToHandlerCache.Count);
      Assert.AreEqual(expectedAllTypesCacheCount, allTypesCache.Count);
      Assert.AreEqual(expectedCompatibleDataTypesCacheCount, compatibleDataTypesCache.Count); */

      Assert.AreEqual(naiveLogLength, tickHandlerLogLength);
      var frac = tickHandlerTimeSpan.TotalMilliseconds / naiveTimeSpan.TotalMilliseconds;
      Assert.IsTrue(frac < 0, $"Was actually {frac}");
    }
  }
}