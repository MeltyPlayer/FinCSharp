using fin.math;

namespace simple.platformer.player {
  /// <summary>
  ///   Simple, common class for managing position, velocity, and acceleration.
  /// </summary>
  public class Rigidbody {
    public double X { get; set; }
    public double Y { get; set; }

    public (double, double) Position {
      get => (this.X, this.Y);
      set => (this.X, this.Y) = value;
    }


    public double XVelocity { get; set; }
    public double YVelocity { get; set; }

    public (double, double) Velocity {
      get => (this.XVelocity, this.YVelocity);
      set => (this.XVelocity, this.YVelocity) = value;
    }


    public double XAcceleration { get; set; }
    public double YAcceleration { get; set; }

    public (double, double) Acceleration {
      get => (this.XAcceleration, this.YAcceleration);
      set => (this.XAcceleration, this.YAcceleration) = value;
    }


    public double MaxXSpeed { get; set; }
    public double MaxYSpeed { get; set; }

    public (double, double) MaxSpeed {
      get => (this.MaxXSpeed, this.MaxYSpeed);
      set => (this.MaxXSpeed, this.MaxYSpeed) = value;
    }


    public double Friction { get; set; }


    public void TickPhysics(int eulerMethodStepCount) {
      var eulerFrac = 1d / eulerMethodStepCount;
      for (var i = 0; i < eulerMethodStepCount; ++i) {
        // TODO: Move this to collision?
        this.XVelocity =
            Math.AddTowards(this.XVelocity, 0, this.Friction * eulerFrac);
        this.XVelocity = Math.Clamp(-this.MaxXSpeed,
                                    this.XVelocity +
                                    this.XAcceleration * eulerFrac,
                                    this.MaxXSpeed);
        this.X += this.XVelocity * eulerFrac;

        this.YVelocity = Math.Clamp(-this.MaxYSpeed,
                                    this.YVelocity +
                                    this.YAcceleration * eulerFrac,
                                    this.MaxYSpeed);
        this.Y += this.YVelocity * eulerFrac;
      }
    }
  }
}