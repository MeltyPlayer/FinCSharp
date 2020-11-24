using fin.math;

namespace simple.platformer.player {
  /// <summary>
  ///   Simple, common class for managing position, velocity, and acceleration.
  /// </summary>
  public class Rigidbody {
    public float X { get; set; }

    public float Y { get; set; }

    public (float, float) Position {
      get => (this.X, this.Y);
      set => (this.X, this.Y) = value;
    }


    public float PreviousX { get; private set; }
    public float PreviousY { get; private set; }

    public (float, float) PreviousPosition {
      get => (this.PreviousX, this.PreviousY);
      private set => (this.PreviousX, this.PreviousY) = value;
    }

    private void UpdatePreviousPosition_() {
      this.PreviousPosition = this.Position;
    }


    public float XVelocity { get; set; }
    public float YVelocity { get; set; }

    public (float, float) Velocity {
      get => (this.XVelocity, this.YVelocity);
      set => (this.XVelocity, this.YVelocity) = value;
    }


    public float? TargetXVelocity { get; set; }

    public float XAcceleration { get; set; }
    //public float XAccelerationFactor { get; set; }

    public float YAcceleration { get; set; }


    public float MaxXSpeed { get; set; }
    public float MaxYSpeed { get; set; }

    public (float, float) MaxSpeed {
      get => (this.MaxXSpeed, this.MaxYSpeed);
      set => (this.MaxXSpeed, this.MaxYSpeed) = value;
    }


    public float Friction { get; set; }


    public void TickPhysics(int eulerMethodStepCount) {
      this.UpdatePreviousPosition_();

      float? nullableClampedTargetXVelocity = null;
      if (this.TargetXVelocity != null) {
        nullableClampedTargetXVelocity =
            FloatMath.Clamp(-this.MaxXSpeed,
                          (float) this.TargetXVelocity,
                          this.MaxXSpeed);
      }

      var eulerFrac = 1f / eulerMethodStepCount;
      for (var i = 0; i < eulerMethodStepCount; ++i) {
        // TODO: Move this to collision?
        this.XVelocity =
            FloatMath.AddTowards(this.XVelocity, 0, this.Friction * eulerFrac);
        if (nullableClampedTargetXVelocity != null) {
          var clampedTargetXVelocity = (float) nullableClampedTargetXVelocity;
          this.XVelocity =
              FloatMath.AddTowards(this.XVelocity,
                                   clampedTargetXVelocity,
                                   FloatMath.Abs(
                                       this.XAcceleration * eulerFrac));
        } else {
          this.XVelocity += this.XAcceleration * eulerFrac;
        }
        this.XVelocity = FloatMath.Clamp(-this.MaxXSpeed,
                                         this.XVelocity,
                                         this.MaxXSpeed);
        this.X += this.XVelocity * eulerFrac;

        this.YVelocity = FloatMath.Clamp(-this.MaxYSpeed,
                                         this.YVelocity +
                                         this.YAcceleration * eulerFrac,
                                         this.MaxYSpeed);
        this.Y += this.YVelocity * eulerFrac;
      }
    }
  }
}