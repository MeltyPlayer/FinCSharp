using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.data.enumerator {
  [TestClass]
  public class AggregateEnumerableTest {
    [TestMethod]
    public void TestEmpty() {
      var enumerable = new AggregateEnumerable<int>();
      Assert.AreEqual(0, enumerable.Count());
    }

    [TestMethod]
    public void TestSingle() {
      var value = new[] {0};
      var enumerable = new AggregateEnumerable<int>(value);

      Assert.AreEqual(1, enumerable.Count());
      Assert.AreEqual(0, enumerable.ToArray()[0]);
    }

    [TestMethod]
    public void TestMultiple() {
      var value = new[] { 0 };
      var empty = Array.Empty<int>();
      var set = new HashSet<int>(new [] {1, 2});
      var enumerable = new AggregateEnumerable<int>(value, empty, set);

      Assert.AreEqual(3, enumerable.Count());

      var values = enumerable.ToArray();
      Assert.AreEqual(0, values[0]);
      Assert.AreEqual(1, values[1]);
      Assert.AreEqual(2, values[2]);
    }

    [TestMethod]
    public void TestChange() {
      var value = new int?[] { 0 };
      var empty = Array.Empty<int?>();
      var set = new HashSet<int?>(new int?[] { 1, 2 });
      var enumerable = new AggregateEnumerable<int?>(value, empty, set);

      using var enumerator = enumerable.GetEnumerator();
      
      value[0] = 3;
      set.Clear();
      set.Add(4);
      set.Add(5);
      set.Add(6);

      Assert.AreEqual(4, enumerable.Count());

      Assert.AreEqual(null, enumerator.Current);
      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual(3, enumerator.Current);
      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual(4, enumerator.Current);
      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual(5, enumerator.Current);
      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual(6, enumerator.Current);
      Assert.AreEqual(false, enumerator.MoveNext());
    }
  }
}