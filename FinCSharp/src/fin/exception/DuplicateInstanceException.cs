using System;

namespace fin.exception {
  public class DuplicateInstanceException : Exception {
    public DuplicateInstanceException() { }

    public DuplicateInstanceException(string message) : base(message) { }

    public DuplicateInstanceException(string message, Exception inner) : base(message, inner) { }
  }
}
