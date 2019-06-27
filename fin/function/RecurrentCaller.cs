using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fin.function {
  public class RecurrentCaller {
    private readonly Action handler_;
    private readonly double frequency_;
    private bool isRunning_;

    public RecurrentCaller(double frequency, Action handler) {
      frequency_ = frequency;
      handler_ = handler;
    }

    // TODO: Add period constructor.

    public void Start() {
      if (isRunning_) {
        return;
      }
      isRunning_ = true;

      double msPerIteration = 1000 / frequency_;

      DateTime previousTime = DateTime.UtcNow;
      while (isRunning_) {
        DateTime currentTime = DateTime.UtcNow;
        TimeSpan frameSpan = currentTime - previousTime;
        double frameMs = frameSpan.TotalMilliseconds;

        handler_();

        // TODO: Keep track of truncated time, add/subtract from next frame?
        int remainingMs = (int)(msPerIteration - frameMs);
        if (remainingMs > 0) {
          Thread.Sleep(remainingMs);
        }
        previousTime = currentTime;
      }
    }

    public void Stop() {
      isRunning_ = false;
    }
  }
}
