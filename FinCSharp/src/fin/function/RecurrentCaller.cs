using System;
using System.Threading;
using fin.log;

namespace fin.function {
  /**
   * Calls a given handler recurrently over time. Assumes the handler will take
   * less time than the prescribed delay between calls.
   */

  public class RecurrentCaller {
    private readonly Action handler_;
    private readonly double frequency_;
    private bool isRunning_;

    public static RecurrentCaller FromFrequency(double frequency, Action handler) {
      return new RecurrentCaller(frequency, handler);
    }

    public static RecurrentCaller FromPeriod(double period, Action handler) {
      return new RecurrentCaller(1 / period, handler);
    }

    private RecurrentCaller(double frequency, Action handler) {
      frequency_ = frequency;
      handler_ = handler;
    }

    public void Start() {
      if (isRunning_) {
        return;
      }
      isRunning_ = true;

      double millisPerIteration = 1000 / frequency_;

      DateTime previousTime = DateTime.UtcNow;
      while (isRunning_) {
        DateTime currentTime = DateTime.UtcNow;
        TimeSpan frameSpan = currentTime - previousTime;
        double frameMillis = frameSpan.TotalMilliseconds;

        handler_();

        // Calculate the remaining time in the iteration, sleep for that # of millis.
        double remainingMillis = millisPerIteration - frameMillis;
        int truncatedRemainingMillis = (int) remainingMillis;
        if (truncatedRemainingMillis > 0) {
          Thread.Sleep(truncatedRemainingMillis);
        }
        else {
          Logger.Log(LogType.PERFORMANCE,
            LogSeverity.WARNING,
            "Recurrent call took too long. Expected " + millisPerIteration +
            "ms but was " + frameMillis + "ms.");
        }

        // TODO: Subtract the float remainder from the current time for more accuracy.
        //double remainderRemainingMillis = remainingMillis - truncatedRemainingMillis;
        //previousTime = currentTime.AddMilliseconds(-remainderRemainingMillis);
        previousTime = currentTime;
      }
    }

    public void Stop() {
      isRunning_ = false;
    }
  }
}