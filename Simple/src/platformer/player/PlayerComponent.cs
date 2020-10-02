using fin.app;
using fin.app.events;
using fin.app.node;
using fin.graphics.camera;
using fin.graphics.color;
using fin.input;
using fin.math;

using simple.platformer.world;

using CColor = System.Drawing.Color;

namespace simple.platformer.player {
  public class PlayerComponent : IComponent {
    private IGamepad gamepad_;

    private readonly LevelGridRenderer levelGridRenderer_;

    private readonly PlayerStateMachine stateMachine_ = new PlayerStateMachine {
        State = PlayerState.STANDING,
    };

    private readonly Rigidbody rigidbody_;
    private readonly PlayerRigidbody playerRigidbody_;

    private readonly PlayerMotor motor_;
    private readonly PlayerCollider collider_;
    private readonly BoxPlayerRenderer boxPlayerRenderer_;

    public PlayerComponent(IGamepad gamepad) {
      this.gamepad_ = gamepad;

      this.levelGridRenderer_ = new LevelGridRenderer {
          LevelGrid = LevelConstants.LEVEL_GRID,
      };

      this.rigidbody_ = new Rigidbody {
          Position = (LevelConstants.SIZE * 10, LevelConstants.SIZE * 13),
          YAcceleration = PlayerConstants.GRAVITY,
          MaxYSpeed = double.MaxValue,
      };
      this.playerRigidbody_ = new PlayerRigidbody {
          Rigidbody = this.rigidbody_,
      };

      this.motor_ = new PlayerMotor {
          StateMachine = this.stateMachine_,
          PlayerRigidbody = this.playerRigidbody_,
      };

      this.collider_ = new PlayerCollider {
          StateMachine = this.stateMachine_,
          PlayerRigidbody = this.playerRigidbody_,
      };

      this.boxPlayerRenderer_ = new BoxPlayerRenderer {
          PlayerRigidbody = this.playerRigidbody_,
      };
    }

    [OnTick]
    private void ProcessInputs_(ProcessInputsEvent _) {
      var primaryAnalogStick = this.gamepad_[AnalogStickType.PRIMARY];
      var heldAxes = primaryAnalogStick.NormalizedAxes;
      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      this.motor_.ScheduleMoveAttempt(heldAxes.X, runButton.IsDown);

      if (heldAxes.Y < -.5) {
        this.motor_.ScheduleDuckStartAttempt();
      }
      else {
        this.motor_.ScheduleDuckStopAttempt();
      }

      var jumpButton = this.gamepad_[FaceButtonType.PRIMARY];
      if (jumpButton.IsPressed) {
        this.motor_.ScheduleJumpStartAttempt();
      }
      else if (jumpButton.IsReleased) {
        this.motor_.ScheduleJumpStopAttempt();
      }

      this.motor_.ProcessInputs();
    }

    [OnTick]
    private void TickPhysics_(TickPhysicsEvent _) {
      var heldX = this.gamepad_[AnalogStickType.PRIMARY].NormalizedAxes.X;
      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      var isRunning = runButton.IsDown;

      var duckedMaxXSpd = isRunning
                              ? PlayerConstants.DUCKED_MAX_FAST_XSPD
                              : PlayerConstants.DUCKED_MAX_SLOW_XSPD;

      // TODO: Wrap these in a struct.
      this.rigidbody_.MaxXSpeed =
          this.stateMachine_.State == PlayerState.SLIDING
              ? PlayerConstants.UPRIGHT_MAX_FAST_XSPD
              : this.stateMachine_.State == PlayerState.LONGJUMPING
                  ? PlayerConstants.LONGJUMP_MAX_XSPD
                  : !this.stateMachine_.IsDucked
                      ? isRunning
                            ? PlayerConstants.UPRIGHT_MAX_FAST_XSPD
                            : PlayerConstants.UPRIGHT_MAX_SLOW_XSPD
                      : duckedMaxXSpd;

      if (this.stateMachine_.IsOnGround) {
        if (this.stateMachine_.State == PlayerState.SLIDING) {
          this.rigidbody_.Friction = PlayerConstants.GROUND_SLIDING_FRICTION;
        }
        else {
          this.rigidbody_.Friction = PlayerConstants.GROUND_FRICTION;
        }
      }
      else {
        this.rigidbody_.Friction = PlayerConstants.AIR_FRICTION;
      }

      var isWide = false; //this.stateMachine_.State == PlayerState.SLIDING;
      var isTall = !this.stateMachine_.IsDucked;
      this.playerRigidbody_.Width =
          !isWide ? PlayerConstants.HSIZE : PlayerConstants.VSIZE;
      this.playerRigidbody_.Height =
          isTall ? PlayerConstants.VSIZE : PlayerConstants.HSIZE;

      this.rigidbody_.TickPhysics(3);

      var (xVelocity, yVelocity) = this.rigidbody_.Velocity;

      // When comes to a stop, start standing.
      if (xVelocity == 0) {
        if (this.stateMachine_.IsMovingUprightOnGround) {
          this.stateMachine_.State = PlayerState.STANDING;
        }
        else if (this.stateMachine_.IsMovingDuckedOnGround) {
          this.stateMachine_.State = PlayerState.DUCKING;
        }
      }

      if (this.stateMachine_.State == PlayerState.SLIDING &&
          Math.Abs(heldX) > .01 &&
          Math.Abs(xVelocity) <= duckedMaxXSpd) {
        this.stateMachine_.State = PlayerState.DUCKWALKING;
      }

      // When transitions to downward y velocity in air after a jump, start
      // falling.
      if (this.stateMachine_.IsMovingUpwardInAirAndCanFall && yVelocity > 0) {
        this.stateMachine_.State = PlayerState.FALLING;
      }
    }

    [OnTick]
    private void TickCollisions_(TickCollisionsEvent _) {
      this.collider_.TickCollisions();

      // If falling while meant to be on the ground, then switch to falling state.
      if (this.stateMachine_.IsOnGround && this.rigidbody_.YVelocity > 0) {
        this.stateMachine_.State = PlayerState.FALLING;
      }
    }

    [OnTick]
    private void TickAnimation_(TickAnimationEvent _) {
      this.boxPlayerRenderer_.Color = this.stateMachine_.State switch {
          PlayerState.STANDING     => CColor.White,
          PlayerState.WALKING      => CColor.Yellow,
          PlayerState.RUNNING      => CColor.Orange,
          PlayerState.TURNING      => CColor.Magenta,
          PlayerState.STOPPING     => CColor.Bisque,
          PlayerState.DUCKING      => CColor.Chartreuse,
          PlayerState.DUCKWALKING  => CColor.ForestGreen,
          PlayerState.SLIDING      => CColor.LightGreen,
          PlayerState.JUMPING      => CColor.Cyan,
          PlayerState.BACKFLIPPING => CColor.Purple,
          PlayerState.LONGJUMPING  => CColor.DodgerBlue,
          PlayerState.FALLING      => CColor.LightBlue,
          _                        => CColor.Black,
      };
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      var g = evt.Graphics;
      this.levelGridRenderer_.Render(g);

      this.boxPlayerRenderer_.Render(g);
    }
  }
}