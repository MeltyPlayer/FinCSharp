using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace fin.assert {
  [TestClass]
  public class AssertsTest {
    [TestClass]
    public class AssertEqualForEnumerables {
      [TestClass]
      public class Valid {
        [TestMethod]
        public void TestEmptyArrays() {
          Asserts.Equal(Array.Empty<object>(), Array.Empty<object>());
        }

        [TestMethod]
        public void TestSingletonArrays() {
          Asserts.Equal(new[] {1,}, new[] {1,});
        }

        [TestMethod]
        public void TestOrderedArrays() {
          Asserts.Equal(new[] {1, 2, 3}, new[] {1, 2, 3});
        }
      }

      [TestClass]
      public class Invalid {
        [TestMethod]
        public void TestDifferentSizedArrays() {
          Assert.ThrowsException<AssertionException>(() =>
            Asserts.Equal(new[] {1, 2}, new[] {1,}));
        }

        [TestMethod]
        public void TestDifferentOrderedArrays() {
          Assert.ThrowsException<AssertionException>(() =>
            Asserts.Equal(new[] {1, 2, 3}, new[] {3, 2, 1}));
        }
      }
    }
  }
}