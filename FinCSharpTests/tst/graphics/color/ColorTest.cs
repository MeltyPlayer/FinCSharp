using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.graphics.color {
  [TestClass]
  public class ColorTest {
    [TestMethod]
    public void TestFromRgbaB() {
      var color = Color.FromRgbaB(1, 2, 3, 4);
      Assert.AreEqual(1, color.Rb);
      Assert.AreEqual(2, color.Gb);
      Assert.AreEqual(3, color.Bb);
      Assert.AreEqual(4, color.Ab);
    }

    [TestMethod]
    public void TestFromRgbaF() {
      var color = Color.FromRgbaF(.1f, .2f, .3f, .4f);
      var tolerance = .01f;
      Assert.AreEqual(.1, color.Rf, tolerance);
      Assert.AreEqual(.2, color.Gf, tolerance);
      Assert.AreEqual(.3, color.Bf, tolerance);
      Assert.AreEqual(.4, color.Af, tolerance);
    }
  }
}