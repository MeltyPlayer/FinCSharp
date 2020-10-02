namespace fin.log {
  public enum LogType {
    PERFORMANCE,
    DEBUG,
  }

  public enum LogSeverity {
    INFO,
    WARNING,
    ERROR,
  }

  public interface ILogger {
    void Log(LogType type, LogSeverity severity, string message);

    void Log(string message) =>
        this.Log(LogType.DEBUG, LogSeverity.INFO, message);
  }
}