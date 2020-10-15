using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.assert {
  public class AssertionException : UnitTestAssertException {
    public AssertionException(string message) : base(message) {}

    public override string StackTrace {
      get {
        List<string> stackTrace = new List<string>();
        stackTrace.AddRange(base.StackTrace!.Split(
                                new string[] {Environment.NewLine},
                                StringSplitOptions.None));
        
        var assertLine = new Regex("\\s*Asserts\\.");
        stackTrace.RemoveAll(x => assertLine.IsMatch(x));
        
        return string.Join(Environment.NewLine, stackTrace.ToArray());
      }
    }
  }
}