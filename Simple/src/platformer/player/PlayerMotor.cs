using fin.math;

using simple.platformer.world;

using CMath = System.Math;

namespace simple.platformer.player {
  /// <summary>
  ///   Common class for performing high-level movement actions for a player.
  ///   Input components should use this rather than directly manipulate the
  ///   rigidbody.
  /// </summary>
  // TODO: Should this also manage collisions?
  public class PlayerMotor {
    public PlayerStateMachine StateMachine { get; set; }
    public PlayerRigidbody PlayerRigidbody { get; set; }
    public Rigidbody Rigidbody => this.PlayerRigidbody.Rigidbody;

    public float HeldXAxis { get; private set; }
    public bool IsRunning { get; private set; }

    private enum ScheduledState {
      UNDEFINED,
      STARTING,
      STOPPING,
    }

    private ScheduledState scheduledDuckState_ = ScheduledState.UNDEFINED;
    private ScheduledState scheduledJumpState_ = ScheduledState.UNDEFINED;

    public void ClearScheduled() {
      this.scheduledDuckState_ = ScheduledState.UNDEFINED;
      this.HeldXAxis = 0;
      this.IsRunning = false;
      this.scheduledJumpState_ = ScheduledState.UNDEFINED;
    }

    /// <summary>
    ///   Schedules an attempt at horizontal movement. This will be performed
    ///   in the next tick if possible.
    /// </summary>
    public void ScheduleMoveAttempt(float heldXAxis, bool isRunning) {
      this.HeldXAxis = heldXAxis;
      this.IsRunning = isRunning;
    }

    public void ScheduleDuckStartAttempt() =>
        this.scheduledDuckState_ = ScheduledState.STARTING;

    public void ScheduleDuckStopAttempt() =>
        this.scheduledDuckState_ = ScheduledState.STOPPING;

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
      // TODO: Move this somewhere else, please.
      var blockSize = LevelConstants.SIZE;
      var aboveLeftX = this.PlayerRigidbody.CenterX - blockSize / 3;
      var aboveRightX = this.PlayerRigidbody.CenterX + blockSize / 3;
      var aboveY = this.PlayerRigidbody.TopY - blockSize / 2;
      var levelGrid = LevelConstants.LEVEL_GRID;

      var isCeilingAbove = levelGrid.CheckAtPosition(aboveLeftX,
                                                     aboveY,
                                                     LevelTileTypes
                                                         .CEILING) ||
                           levelGrid.CheckAtPosition(
                               aboveRightX,
                               aboveY,
                               LevelTileTypes.CEILING);
      if (this.scheduledDuckState_ == ScheduledState.STARTING ||
          !isCeilingAbove) {
        this.ProcessScheduledDuck_();
      }

      this.ProcessScheduledHorizontalMovement_();

      if (!isCeilingAbove) {
        this.ProcessScheduledJump_();
      }
    }

    private void ProcessScheduledDuck_() {
      if (this.StateMachine.CanDuck &&
          this.scheduledDuckState_ == ScheduledState.STARTING) {
        if (this.StateMachine.State == PlayerState.RUNNING) {
          this.StateMachine.State = PlayerState.SLIDING;
        } else if (this.StateMachine.IsMovingUprightOnGround) {
          this.StateMachine.State = PlayerState.DUCKWALKING;
        } else if (this.StateMachine.CanMoveUprightOnGround) {
          this.StateMachine.State = PlayerState.DUCKING;
        }
      } else if (this.scheduledDuckState_ == ScheduledState.STOPPING) {
        if (this.StateMachine.State == PlayerState.DUCKING) {
          this.StateMachine.State = PlayerState.STANDING;
        } else if (this.StateMachine.State == PlayerState.DUCKWALKING) {
          this.StateMachine.State = PlayerState.WALKING;
        } else if (this.StateMachine.State == PlayerState.SLIDING) {
          this.StateMachine.State = PlayerState.STOPPING;
        }
      }
    }

    private void ProcessScheduledHorizontalMovement_() {
      var heldXAxis = this.HeldXAxis;
      var isRunning = this.IsRunning;
      var isTryingToRun = FloatMath.Abs(heldXAxis) > .5f;

      var heldXAxisSign = FinMath.Sign(heldXAxis);

      float? targetXVelocity = null;
      float xAcceleration = 0;

      if (this.StateMachine.CanMoveUprightOnGround) {
        var maxGroundXVelocity =
            isRunning
                ? PlayerConstants.UPRIGHT_MAX_FAST_XSPD
                : PlayerConstants.UPRIGHT_MAX_SLOW_XSPD;
        var groundAcceleration =
            isTryingToRun
                ? PlayerConstants.GROUND_UPRIGHT_FAST_XACC
                : PlayerConstants.GROUND_UPRIGHT_SLOW_XACC;
        var reactionFraction =
            heldXAxisSign == -FinMath.Sign(this.Rigidbody.XVelocity)
                ? PlayerConstants.GROUND_REACTION_FRAC
                : 1;

        targetXVelocity = maxGroundXVelocity * heldXAxis;
        xAcceleration = groundAcceleration * reactionFraction * heldXAxisSign;

        // If holding a direction on the ground, we're either turning, running, or walking.
        if (heldXAxisSign != 0) {
          this.StateMachine.State = reactionFraction != 1
                                        ? PlayerState.TURNING
                                        : isTryingToRun
                                            ? PlayerState.RUNNING
                                            : PlayerState.WALKING;
        }
        // If not holding a direction on the ground but velocity is not zero, we're stopping.
        else if (FinMath.Abs(this.Rigidbody.XVelocity) > .001) {
          this.StateMachine.State = PlayerState.STOPPING;
        }
      } else if (this.StateMachine.CanMoveDuckedOnGround) {
        var maxGroundXVelocity =
            isRunning
                ? PlayerConstants.DUCKED_MAX_FAST_XSPD
                : PlayerConstants.DUCKED_MAX_SLOW_XSPD;
        var groundAcceleration =
            isTryingToRun
                ? PlayerConstants.GROUND_DUCKED_FAST_XACC
                : PlayerConstants.GROUND_DUCKED_SLOW_XACC;

        targetXVelocity = maxGroundXVelocity * heldXAxis;
        xAcceleration = groundAcceleration * heldXAxisSign;

        // If holding a direction on the ground, we're either turning, running, or walking.
        if (heldXAxisSign != 0) {
          this.StateMachine.State = PlayerState.DUCKWALKING;
        }
      } else if (this.StateMachine.CanMoveInAir) {
        var maxAirXVelocity =
            isRunning
                ? PlayerConstants.UPRIGHT_MAX_FAST_XSPD
                : PlayerConstants.UPRIGHT_MAX_SLOW_XSPD;
        var airAcceleration =
            isTryingToRun
                ? PlayerConstants.AIR_FAST_XACC
                : PlayerConstants.AIR_SLOW_XACC;

        targetXVelocity = maxAirXVelocity * heldXAxis;
        xAcceleration = airAcceleration * heldXAxisSign;
      }

      this.Rigidbody.TargetXVelocity = targetXVelocity;
      this.Rigidbody.XAcceleration = xAcceleration;
    }

    private void ProcessScheduledJump_() {
      if (this.scheduledJumpState_ == ScheduledState.STARTING) {
        if (this.StateMachine.CanJumpFromGround) {
          var isLongjump = this.StateMachine.State == PlayerState.SLIDING;
          var isBackflip = this.StateMachine.State == PlayerState.TURNING &&
                           FinMath.Abs(this.Rigidbody.XVelocity) >
                           PlayerConstants.UPRIGHT_MAX_SLOW_XSPD;
          var newYVel =
              isLongjump
                  ? PlayerConstants.LONGJUMP_SPEED
                  : isBackflip
                      ? PlayerConstants.BACKFLIP_JUMP_SPEED
                      : PlayerConstants.JUMP_SPEED;
          var newState =
              isLongjump ? PlayerState.LONGJUMPING :
              isBackflip ? PlayerState.BACKFLIPPING : PlayerState.JUMPING;

          this.Rigidbody.YVelocity = newYVel;
          this.StateMachine.State = newState;

          // TODO: Longjump feels weird.
          if (isLongjump) {
            this.Rigidbody.XVelocity = this.Rigidbody.XVelocity /
                                       PlayerConstants.UPRIGHT_MAX_FAST_XSPD *
                                       PlayerConstants.LONGJUMP_MAX_XSPD;
          }
          // TODO: Backflip feels weird.
          else if (isBackflip) {
            // Instantly flip horizontal velocity.
            var moveDirection = FinMath.Sign(this.Rigidbody.XVelocity);
            this.Rigidbody.XVelocity =
                -moveDirection * PlayerConstants.UPRIGHT_MAX_SLOW_XSPD;
          }
        } else if (this.StateMachine.State == PlayerState.WALL_SLIDING) {
          this.StateMachine.State = PlayerState.WALLJUMPING;
          this.Rigidbody.YVelocity = PlayerConstants.JUMP_SPEED;
          this.Rigidbody.XVelocity =
              (this.StateMachine.WallSlidingOnLeft ? 1 : -1) *
              PlayerConstants.WALL_SLIDING_XSPD;
        }
      } else if (this.scheduledJumpState_ == ScheduledState.STOPPING) {
        if (this.StateMachine.CanStallJumpingMomentum &&
            this.Rigidbody.YVelocity < 0) {
          this.StateMachine.State = PlayerState.FALLING;
          this.Rigidbody.YVelocity /= 2;
        }
      }
    }
  }
}