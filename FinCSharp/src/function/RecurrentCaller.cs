using System;

using RSG;

namespace fin.function {
  public interface IRecurrentCaller {
    double TargetedFrequency { get; }
    double ActualFrequency { get; }

    bool IsRunning { get; }
    void Start();
    IPromise Pause();
  }

  /**
   * Calls a given handler recurrently over time. Assumes the handler will take
   * less time than the prescribed delay between calls.
   */
  public class RecurrentCaller : IRecurrentCaller {
    private readonly Action handler_;
    private IPromise nextPausePromise_ = Promise.Resolved();

    public static RecurrentCaller FromFrequency(double frequency,
                                                Action handler)
      => new RecurrentCaller(frequency, handler);

    public static RecurrentCaller FromPeriod(double period, Action handler) =>
        new RecurrentCaller(1 / period, handler);

    private RecurrentCaller(double targetedFrequency, Action handler) {
      this.TargetedFrequency = targetedFrequency;
      this.handler_ = handler;
    }

    public double TargetedFrequency { get; }
    public double ActualFrequency { get; private set; }

    public bool IsRunning { get; private set; }
 
    public void Start() {
      if (this.IsRunning) {
        return;
      }

      this.IsRunning = true;

      var currentPromise = new Promise();
      this.nextPausePromise_ = currentPromise;

      var millisPerIteration = 1000 / this.TargetedFrequency;

      var previousTime = DateTime.UtcNow;
      while (this.IsRunning) {
        var currentTime = DateTime.UtcNow;
        var frameSpan = currentTime - previousTime;
        var frameMillis = frameSpan.TotalMilliseconds;

        this.handler_();

        // Calculate the remaining time in the iteration, sleep for that # of millis.
        var remainingMillis = millisPerIteration - frameMillis;
        var truncatedRemainingMillis = (int) remainingMillis;

        if (truncatedRemainingMillis > 0) {
          /*Logger.Log(LogType.PERFORMANCE,
            LogSeverity.WARNING,
            "! Time: " + frameMillis + "ms, Rem.: " + remainingMillis + "ms" +
            ", Trun.: " + truncatedRemainingMillis + "ms");

          // TODO: Find a way to sleep without breaking vsync.
          //Thread.Sleep(truncatedRemainingMillis);
        }
        else {
          /*Logger.Log(LogType.PERFORMANCE,
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

      // Only reaches here once stopped.
      currentPromise.Resolve();
    }

    public IPromise Pause() {
      if (this.IsRunning) {
        this.IsRunning = false;
      }
      return this.nextPausePromise_;
    }
  }
}