using Math = fin.math.Math;

namespace simple.platformer.player {
  /// <summary>
  ///   Common class for performing high-level movement actions for a player.
  ///   Input components should use this rather than directly manipulate the
  ///   rigidbody.
  /// </summary>
  // TODO: Should this also manage collisions?
  public class PlayerMotor {
    public PlayerStateMachine StateMachine { get; set; }
    public Rigidbody Rigidbody { get; set; }

    private double scheduledHeldXAxis_;
    private bool scheduledIsRunning_;

    private enum ScheduledState {
      UNDEFINED,
      STARTING,
      STOPPING,
    }

    private ScheduledState scheduledDuckState_ = ScheduledState.UNDEFINED;
    private ScheduledState scheduledJumpState_ = ScheduledState.UNDEFINED;

    public void ScheduleDuckStartAttempt() =>
        this.scheduledDuckState_ = ScheduledState.STARTING;

    public void ScheduleDuckStopAttempt() =>
        this.scheduledDuckState_ = ScheduledState.STOPPING;

    /// <summary>
    ///   Schedules an attempt at horizontal movement. This will be performed
    ///   in the next tick if possible.
    /// </summary>
    public void ScheduleMoveAttempt(double heldXAxis, bool isRunning) {
      this.scheduledHeldXAxis_ = heldXAxis;
      this.scheduledIsRunning_ = isRunning;
    }

    /// <summary>
    ///   Schedules an attempt to start a jump. This will be performed in the
    ///   next tick if possible.
    /// </summary>
    // TODO: Support early/late jumps so it feels more responsive?
    public void ScheduleJumpStartAttempt() =>
        this.scheduledJumpState_ = ScheduledState.STARTING;

    /// <summary>
    ///   Schedules an attempt to stop a jump early. This will be performed in
    ///   the next tick if possible. 
    /// </summary>
    public void ScheduleJumpStopAttempt() =>
        this.scheduledJumpState_ = ScheduledState.STOPPING;

    public void ProcessInputs() {
      this.ProcessScheduledDuck_();
      this.ProcessScheduledHorizontalMovement_();
      this.ProcessScheduledJump_();

      this.scheduledHeldXAxis_ = 0;
      this.scheduledIsRunning_ = false;
      this.scheduledDuckState_ = ScheduledState.UNDEFINED;
      this.scheduledJumpState_ = ScheduledState.UNDEFINED;
    }

    private void ProcessScheduledDuck_() {
      if (this.scheduledDuckState_ == ScheduledState.STARTING) {
        if (this.StateMachine.State == PlayerState.RUNNING) {
          this.StateMachine.State = PlayerState.SLIDING;
          this.Rigidbody.XAcceleration = 0;
        }
        if (this.StateMachine.CanMoveOnGround) {
          this.StateMachine.State = PlayerState.DUCKING;
          this.Rigidbody.XAcceleration = 0;
        }
      }
      else if (this.scheduledDuckState_ == ScheduledState.STOPPING) {
        if (this.StateMachine.State == PlayerState.DUCKING) {
          this.StateMachine.State = PlayerState.STANDING;
        }
        if (this.StateMachine.State == PlayerState.SLIDING) {
          this.StateMachine.State = PlayerState.STOPPING;
        }
      }
    }

    private void ProcessScheduledHorizontalMovement_() {
      var heldXAxis = this.scheduledHeldXAxis_;
      var isRunning = this.scheduledIsRunning_;

      if (this.StateMachine.CanMoveOnGround) {
        var heldXAxisSign = Math.Sign(heldXAxis);

        var groundAcceleration =
            isRunning
                ? PlayerConstants.GROUND_FAST_XACC
                : PlayerConstants.GROUND_SLOW_XACC;
        var reactionFraction =
            heldXAxisSign == -Math.Sign(this.Rigidbody.XVelocity)
                ? PlayerConstants.GROUND_REACTION_FRAC
                : 1;

        this.Rigidbody.XAcceleration =
            groundAcceleration * reactionFraction * heldXAxis;

        // If holding a direction on the ground, we're either turning, running, or walking.
        if (heldXAxisSign != 0) {
          this.StateMachine.State = reactionFraction != 1
                                        ? PlayerState.TURNING
                                        : isRunning
                                            ? PlayerState.RUNNING
                                            : PlayerState.WALKING;
        }
        // If not holding a direction on the ground but velocity is not zero, we're stopping.
        else if (Math.Abs(this.Rigidbody.XVelocity) > .001) {
          this.StateMachine.State = PlayerState.STOPPING;
        }
      }

      if (this.StateMachine.CanMoveInAir) {
        var airAcceleration =
            this.scheduledIsRunning_
                ? PlayerConstants.AIR_FAST_XACC
                : PlayerConstants.AIR_SLOW_XACC;
        this.Rigidbody.XAcceleration = airAcceleration * heldXAxis;
      }
    }

    private void ProcessScheduledJump_() {
      if (this.scheduledJumpState_ == ScheduledState.STARTING) {
        if (this.StateMachine.IsOnGround) {
          var isBackflip = this.StateMachine.State == PlayerState.TURNING &&
                           Math.Abs(this.Rigidbody.XVelocity) >
                           PlayerConstants.MAX_SLOW_XSPD;
          var newYVel =
              isBackflip
                  ? PlayerConstants.BACKFLIP_JUMP_SPEED
                  : PlayerConstants.JUMP_SPEED;
          var newState =
              isBackflip ? PlayerState.BACKFLIPPING : PlayerState.JUMPING;

          this.Rigidbody.YVelocity = newYVel;
          this.StateMachine.State = newState;

          if (isBackflip) {
            // Instantly flip horizontal velocity.
            var moveDirection = Math.Sign(this.Rigidbody.XVelocity);
            this.Rigidbody.XVelocity =
                -moveDirection * PlayerConstants.MAX_SLOW_XSPD;
          }
        }
      }
      else if (this.scheduledJumpState_ == ScheduledState.STOPPING) {
        if (this.StateMachine.IsJumping) {
          this.StateMachine.State = PlayerState.FALLING;
          this.Rigidbody.YVelocity /= 2;
        }
      }
    }
  }
}