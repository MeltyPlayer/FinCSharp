using System;

namespace fin.log {
  public class ConsoleLogger : ILogger {
    public void Log(LogType type, LogSeverity severity, string message)
      => Console.WriteLine(type + ", " + severity + ": " + message);
  }
}