using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;

namespace fin.pointer.contract {

  public static class Utils {

    public static void AssertContents<T>(IContractOwner<T> set, params IContractPointer<T>[] contracts) {
      CollectionAssert.AreEqual(contracts, set.Contracts.ToArray());
    }

    public static void AssertEmpty<T>(IContractOwner<T> set) {
      AssertContents(set);
    }
  }
}