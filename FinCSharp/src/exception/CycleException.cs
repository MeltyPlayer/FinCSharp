using System;

namespace fin.exception {
  public class CycleException : Exception {
    public CycleException() { }

    public CycleException(string message) : base(message) { }

    public CycleException(string message, Exception inner)
      : base(message, inner) { }
  }
}