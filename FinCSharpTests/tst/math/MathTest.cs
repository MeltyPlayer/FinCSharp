using fin.math;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.math {
  [TestClass]
  public class MathTest {
    /**
     * Operations
     */
    [TestMethod]
    public void TestMod() {
      Assert.AreEqual(0, FinMath.Mod(10, 2));
      Assert.AreEqual(1, FinMath.Mod(10, 3));

      Assert.AreEqual(1L, FinMath.Mod(10L, 3L));

      Assert.AreEqual(.111, FinMath.Mod(.234, .123), .0001);

      Assert.AreEqual(0x01, FinMath.Mod(0x10, 0x03));
    }

    /**
     * Working w/ signs
     */
    [TestMethod]
    public void TestAbs() {
      Assert.AreEqual(5, FinMath.Abs(5));
      Assert.AreEqual(5, FinMath.Abs(-5));

      Assert.AreEqual(5d, FinMath.Abs(5d));
      Assert.AreEqual(5d, FinMath.Abs(-5d));
    }

    [TestMethod]
    public void TestSign() {
      Assert.AreEqual(-1, FinMath.Sign(-1));
      Assert.AreEqual(1, FinMath.Sign(1));
      Assert.AreEqual(0, FinMath.Sign(0));

      Assert.AreEqual(-1, FinMath.Sign(-1d));
      Assert.AreEqual(1, FinMath.Sign(1d));
      Assert.AreEqual(0, FinMath.Sign(0d));
    }

    /**
     * Applying value ranges
     */
    [TestMethod]
    public void TestMin() {
      Assert.AreEqual(0, FinMath.Min(0, 1));
      Assert.AreEqual(0, FinMath.Min(1, 0));

      Assert.AreEqual(1L, FinMath.Min(1L, 2L));

      Assert.AreEqual(.123, FinMath.Min(.123, .234));

      Assert.AreEqual(0x01, FinMath.Min(0x01, 0x02));
    }

    [TestMethod]
    public void TestMax() {
      Assert.AreEqual(1, FinMath.Max(0, 1));
      Assert.AreEqual(1, FinMath.Max(1, 0));

      Assert.AreEqual(2L, FinMath.Max(1L, 2L));

      Assert.AreEqual(.234, FinMath.Max(.123, .234));

      Assert.AreEqual(0x02, FinMath.Max(0x01, 0x02));
    }

    [TestMethod]
    public void TestClamp() {
      Assert.AreEqual(0, FinMath.Clamp(-1, 0, 1));
      Assert.AreEqual(-1, FinMath.Clamp(-1, -1, 1));
      Assert.AreEqual(1, FinMath.Clamp(-1, 1, 1));
      Assert.AreEqual(1, FinMath.Clamp(-1, 2, 1));
      Assert.AreEqual(-1, FinMath.Clamp(-1, -2, 1));

      Assert.AreEqual(1L, FinMath.Clamp(0L, 2L, 1L));

      Assert.AreEqual(.234, FinMath.Clamp(.123, 456d, .234));

      Assert.AreEqual(-0x01, FinMath.Clamp(-0x01, -0x10, 0x01));
    }

    [TestMethod]
    public void TestWrap() {
      Assert.AreEqual(0, FinMath.Wrap(-1, 0, 1));
      Assert.AreEqual(-1, FinMath.Wrap(-1, -1, 1));
      Assert.AreEqual(-1, FinMath.Wrap(-1, 1, 1));
      Assert.AreEqual(-1, FinMath.Wrap(-2, 3, 2));
      Assert.AreEqual(1, FinMath.Wrap(-2, -3, 2));

      Assert.AreEqual(1L, FinMath.Wrap(1L, 3L, 2L));

      Assert.AreEqual(.124, FinMath.Wrap(.123, .235, .234), .0001);

      Assert.AreEqual(0x01, FinMath.Wrap(-0x02, -0x03, 0x02));
    }

    [TestMethod]
    public void TestAddTowards() {
      Assert.AreEqual(.1, FinMath.AddTowards(0, 1, .1));
      Assert.AreEqual(.9, FinMath.AddTowards(1, 0, .1));
      Assert.AreEqual(1, FinMath.AddTowards(0, 1, 1.1));
    }
  }
}