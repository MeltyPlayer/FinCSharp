using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.generic {

  [TestClass]
  public class DivergingSubclassHandlerMapTest {

    public abstract class Base { }

    public class AParam { }

    public abstract class A : Base {

      public void CallA(AParam param) {
      }
    }

    public class BParam { }

    public abstract class B : Base {

      public void CallB(BParam param) {
      }
    }

    [TestMethod]
    public void TestAddSubclasses() {
      var map = new DivergingSubclassHandlerMap<Base>();
      Assert.Fail();
    }
  }
}