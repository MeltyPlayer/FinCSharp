using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections;
using System.Linq;

namespace fin.pointer.contract {
  public static class Utils {
    public static void AssertContents<T>(IContractPointerOwner<T> set,
      params IContractPointer<T>[] contracts) {
      CollectionAssert.AreEqual(contracts, set.Contracts.ToArray());
    }

    public static void AssertEmpty<T>(IContractPointerOwner<T> set) {
      AssertContents(set);
    }
  }
}