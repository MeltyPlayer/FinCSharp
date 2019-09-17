using System;
using System.Threading;
using fin.log;

namespace fin.function {
  /**
   * Calls a given handler recurrently over time. Assumes the handler will take
   * less time than the prescribed delay between calls.
   */

  public class RecurrentCaller {
    public double TargetedFrequency { get; }

    public double ActualFrequency { get; private set; }

    private readonly Action handler_;
    private bool isRunning_;

    public static RecurrentCaller FromFrequency(double frequency, Action handler)
      => new RecurrentCaller(frequency, handler);

    public static RecurrentCaller FromPeriod(double period, Action handler) =>
      new RecurrentCaller(1 / period, handler);

    private RecurrentCaller(double targetedFrequency, Action handler) {
      this.TargetedFrequency = targetedFrequency;
      this.handler_ = handler;
    }

    public void Start() {
      if (this.isRunning_) {
        return;
      }
      this.isRunning_ = true;

      var millisPerIteration = 1000 / this.TargetedFrequency;

      var previousTime = DateTime.UtcNow;
      while (this.isRunning_) {
        var currentTime = DateTime.UtcNow;
        var frameSpan = currentTime - previousTime;
        var frameMillis = frameSpan.TotalMilliseconds;

        this.handler_();

        // Calculate the remaining time in the iteration, sleep for that # of millis.
        var remainingMillis = millisPerIteration - frameMillis;
        var truncatedRemainingMillis = (int) remainingMillis;

        if (truncatedRemainingMillis > 0) {
          Logger.Log(LogType.PERFORMANCE,
            LogSeverity.WARNING,
            "! Time: " + frameMillis + "ms, Rem.: " + remainingMillis + "ms" +
            ", Trun.: " + truncatedRemainingMillis + "ms");

          // TODO: Find a way to sleep without breaking vsync.
          //Thread.Sleep(truncatedRemainingMillis);
        }
        else {
          Logger.Log(LogType.PERFORMANCE,
            LogSeverity.WARNING,
            ". Time: " + frameMillis + "ms, Rem.: " + remainingMillis + "ms" +
            ", Trun.: " + truncatedRemainingMillis + "ms");
          // TODO: Log slowness?
          /* Logger.Log(LogType.PERFORMANCE,
            LogSeverity.WARNING,
            "Recurrent call took too long. Expected " + millisPerIteration +
            "ms but was " + frameMillis + "ms."); */
        }

        // TODO: Subtract the float remainder from the current time for more accuracy.
        //var remainderRemainingMillis = remainingMillis - truncatedRemainingMillis;
        //previousTime = currentTime.AddMilliseconds(remainderRemainingMillis);
        previousTime = currentTime;
        this.ActualFrequency = 1 / (frameMillis / 1000);
      }
    }

    public void Stop() {
      this.isRunning_ = false;
    }
  }
}