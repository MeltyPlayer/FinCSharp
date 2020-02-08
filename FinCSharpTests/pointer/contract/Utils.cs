using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;

namespace fin.pointer.contract {

  public static class Utils {

    public static IContractOwner<T>[] NewArray<T>(params IContractOwner<T>[] owners) {
      return owners;
    }

    public static void AssertContents<T>(IStrongContractSet<T> set, params IContractPointer<T>[] contracts) {
      CollectionAssert.AreEqual(contracts, set.Contracts.ToArray());
    }

    public static void AssertContents<T>(IWeakContractSet<T> set, params IContractPointer<T>[] contracts) {
      CollectionAssert.AreEqual(contracts, set.Contracts.ToArray());
    }

    public static void AssertEmpty<T>(IStrongContractSet<T> set) {
      AssertContents(set);
    }

    public static void AssertEmpty<T>(IWeakContractSet<T> set) {
      AssertContents(set);
    }
  }
}