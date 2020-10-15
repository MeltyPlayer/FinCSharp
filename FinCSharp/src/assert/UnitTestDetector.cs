using System;
using System.Reflection;

namespace fin.assert {
  /// <summary>
  /// Detect if we are running as part of a nUnit unit test.
  /// This is DIRTY and should only be used if absolutely necessary as its
  /// usually a sign of bad design.
  /// From https://stackoverflow.com/questions/3167617/determine-if-code-is-running-as-part-of-a-unit-test
  /// </summary>
  public static class UnitTestDetector {
    static UnitTestDetector() {
      foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies()) {
        // Can't do something like this as it will load the nUnit assembly
        // if (assem == typeof(NUnit.Framework.Assert))

        if (assem.FullName == null) {
          continue;
        }

        var fullName = assem.FullName.ToLowerInvariant();
        if (fullName.StartsWith(
                "microsoft.visualStudio.testTools.unitTesting") ||
            fullName.StartsWith("nunit.framework")) {
          UnitTestDetector.IsRunningFromNUnit = true;
          break;
        }
      }
    }

    public static bool IsRunningFromNUnit { get; } = false;
  }
}