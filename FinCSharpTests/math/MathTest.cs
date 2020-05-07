using fin.math;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.math {
  [TestClass]
  public class MathTest {
    [TestMethod]
    public void TestMod() {
      Assert.AreEqual(0, Math.Mod(10, 2));
      Assert.AreEqual(1, Math.Mod(10, 3));

      Assert.AreEqual(1L, Math.Mod(10L, 3L));

      Assert.AreEqual(.111, Math.Mod(.234, .123), .0001);

      Assert.AreEqual(0x01, Math.Mod(0x10, 0x03));
    }
    
    [TestMethod]
    public void TestMin() {
      Assert.AreEqual(0, Math.Min(0, 1));
      Assert.AreEqual(0, Math.Min(1, 0));

      Assert.AreEqual(1L, Math.Min(1L, 2L));

      Assert.AreEqual(.123, Math.Min(.123, .234));

      Assert.AreEqual(0x01, Math.Min(0x01, 0x02));
    }

    [TestMethod]
    public void TestMax() {
      Assert.AreEqual(1, Math.Max(0, 1));
      Assert.AreEqual(1, Math.Max(1, 0));

      Assert.AreEqual(2L, Math.Max(1L, 2L));

      Assert.AreEqual(.234, Math.Max(.123, .234));

      Assert.AreEqual(0x02, Math.Max(0x01, 0x02));
    }

    [TestMethod]
    public void TestClamp() {
      Assert.AreEqual(0, Math.Clamp(-1, 0, 1));
      Assert.AreEqual(-1, Math.Clamp(-1, -1, 1));
      Assert.AreEqual(1, Math.Clamp(-1, 1, 1));
      Assert.AreEqual(1, Math.Clamp(-1, 2, 1));
      Assert.AreEqual(-1, Math.Clamp(-1, -2, 1));

      Assert.AreEqual(1L, Math.Clamp(0L, 2L, 1L));

      Assert.AreEqual(.234, Math.Clamp(.123, 456d, .234));

      Assert.AreEqual(-0x01, Math.Clamp(-0x01, -0x10, 0x01));
    }

    [TestMethod]
    public void TestWrap() {
      Assert.AreEqual(0, Math.Wrap(-1, 0, 1));
      Assert.AreEqual(-1, Math.Wrap(-1, -1, 1));
      Assert.AreEqual(-1, Math.Wrap(-1, 1, 1));
      Assert.AreEqual(-1, Math.Wrap(-2, 3, 2));
      Assert.AreEqual(1, Math.Wrap(-2, -3, 2));

      Assert.AreEqual(1L, Math.Wrap(1L, 3L, 2L));

      Assert.AreEqual(.124, Math.Wrap(.123, .235, .234), .0001);

      Assert.AreEqual(0x01, Math.Wrap(-0x02, -0x03, 0x02));
    }
  }
}