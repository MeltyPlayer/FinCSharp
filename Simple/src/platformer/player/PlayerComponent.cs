using System;
using System.Drawing;

using fin.app;
using fin.app.events;
using fin.app.node;
using fin.audio;
using fin.graphics.camera;
using fin.graphics.color;
using fin.input;
using fin.input.gamepad;
using fin.math;

using simple.platformer.player.sfx;
using simple.platformer.world;

namespace simple.platformer.player {
  public class PlayerComponent : IComponent {
    private readonly IGamepad gamepad_;
    private readonly PlayerRigidbody playerRigidbody_;
    private readonly Rigidbody rigidbody_;
    private readonly PlayerSoundsComponent playerSounds_;
    private readonly PlayerStateMachine stateMachine_;

    private readonly LevelGridRenderer levelGridRenderer_;

    private readonly PlayerCollider collider_;
    private readonly PlayerMotor motor_;
    private readonly BoxPlayerRenderer boxPlayerRenderer_;

    private float duckFraction_ = 0;
    private float toDuckFraction_ = 0;

    public PlayerComponent(
        IGamepad gamepad,
        PlayerRigidbody playerRigidbody,
        PlayerSoundsComponent playerSounds,
        PlayerStateMachine playerStateMachine) {
      this.gamepad_ = gamepad;

      this.playerRigidbody_ = playerRigidbody;
      this.rigidbody_ = playerRigidbody.Rigidbody;

      this.playerSounds_ = playerSounds;
      this.stateMachine_ = playerStateMachine;

      this.levelGridRenderer_ = new LevelGridRenderer {
          LevelGrid = LevelConstants.LEVEL_GRID,
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
      this.motor_.ClearScheduled();

      var primaryAnalogStick = this.gamepad_[AnalogStickType.PRIMARY];
      var heldAxes = primaryAnalogStick.RawAxes;
      //var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      var isRunning = true;

      var heldX = FinMath.Abs(heldAxes.X) > GamepadConstants.DEADZONE
                      ? heldAxes.X
                      : 0;
      this.motor_.ScheduleMoveAttempt(heldX, isRunning);

      var heldY = heldAxes.Y;
      var minHeldDuckAmount = -.75f;
      if (this.stateMachine_.CanDuck) {
        var maxHeldDuckAmount = -.25f;
        this.toDuckFraction_ = 1 -
                               (FloatMath.Clamp(minHeldDuckAmount,
                                                heldY,
                                                maxHeldDuckAmount) -
                                minHeldDuckAmount) /
                               (maxHeldDuckAmount - minHeldDuckAmount);
      } else if (this.stateMachine_.IsDucked) {
        this.toDuckFraction_ = 1;
      } else {
        this.toDuckFraction_ = 0;
      }

      this.duckFraction_ += (this.toDuckFraction_ - this.duckFraction_) / 2;

      if (heldY <= minHeldDuckAmount) {
        this.motor_.ScheduleDuckStartAttempt();
      } else {
        this.motor_.ScheduleDuckStopAttempt();
      }

      var jumpButton = this.gamepad_[FaceButtonType.PRIMARY];
      if (jumpButton.IsPressed) {
        this.motor_.ScheduleJumpStartAttempt();
      } else if (jumpButton.IsReleased) {
        this.motor_.ScheduleJumpStopAttempt();
      }

      this.motor_.ProcessInputs();
    }

    [OnTick]
    private void TickPhysics_(TickPhysicsEvent _) {
      var heldX = this.motor_.HeldXAxis;
      var isRunning = this.motor_.IsRunning;

      // TODO: Wrap these in a struct.
      this.rigidbody_.MaxXSpeed = float.MaxValue;
      var duckedMaxXSpd = isRunning
                              ? PlayerConstants.DUCKED_MAX_FAST_XSPD
                              : PlayerConstants.DUCKED_MAX_SLOW_XSPD;

      if (this.stateMachine_.IsOnGround) {
        if (this.stateMachine_.State == PlayerState.SLIDING) {
          this.rigidbody_.Friction = PlayerConstants.GROUND_SLIDING_FRICTION;
        } else {
          this.rigidbody_.Friction = PlayerConstants.GROUND_FRICTION;
        }
      } else {
        this.rigidbody_.Friction = PlayerConstants.AIR_FRICTION;
      }

      var isWide = false; //this.stateMachine_.State == PlayerState.SLIDING;
      var isTall = !this.stateMachine_.IsDucked;
      // TODO: Might be a problem, this should probably just be a visible thing.
      // TODO: Might be a problem, this should probably just be a visible thing.
      this.playerRigidbody_.Width =
          (.8f * PlayerConstants.HSIZE) * (1 - this.duckFraction_) +
          PlayerConstants.HSIZE * this.duckFraction_;
      this.playerRigidbody_.Height =
          PlayerConstants.VSIZE * (1 - this.duckFraction_) +
          PlayerConstants.HSIZE * this.duckFraction_;
      //this.playerRigidbody_.Width =
      // !isWide ? PlayerConstants.HSIZE : PlayerConstants.VSIZE;
      //
      //this.playerRigidbody_.Height =
      //    isTall ? PlayerConstants.VSIZE : PlayerConstants.HSIZE;

      this.rigidbody_.TickPhysics(3);

      var (xVelocity, yVelocity) = this.rigidbody_.Velocity;

      // When comes to a stop, start standing.
      if (xVelocity == 0) {
        if (this.stateMachine_.IsMovingUprightOnGround) {
          this.stateMachine_.State = PlayerState.STANDING;
        } else if (this.stateMachine_.IsMovingDuckedOnGround) {
          this.stateMachine_.State = PlayerState.DUCKING;
        }
      }

      if (this.stateMachine_.State == PlayerState.SLIDING &&
          FinMath.Abs(heldX) > GamepadConstants.DEADZONE &&
          FinMath.Abs(xVelocity) <= duckedMaxXSpd) {
        this.stateMachine_.State = PlayerState.DUCKWALKING;
      }

      // When transitions to downward y velocity in air after a jump, start
      // falling.
      if (this.stateMachine_.IsMovingUpwardInAirAndCanFall && yVelocity > 0) {
        this.stateMachine_.State = PlayerState.FALLING;
      }

      if (this.stateMachine_.State == PlayerState.INITIALLY_FALLING_OFF_LEDGE &&
          --this.initiallyFallingTimer_ <= -1) {
        this.stateMachine_.State = PlayerState.FALLING;
      }
    }

    // TODO: Move this into a stateMachine behavior.
    private int initiallyFallingTimer_ = -1;

    [OnTick]
    private void TickCollisions_(TickCollisionsEvent _) {
      var initXSpd = FloatMath.Abs(this.playerRigidbody_.XVelocity);
      var collidedTypes = this.collider_.TickCollisions();

      if ((collidedTypes & LevelTileTypes.LEFT_WALL) != 0 ||
          (collidedTypes & LevelTileTypes.RIGHT_WALL) != 0) {
        if (initXSpd > 1) {
          this.playerSounds_.PlayBumpWallSound();
        }
      }

      // If falling while meant to be on the ground, then switch to falling state.
      if (this.stateMachine_.IsOnGround && this.rigidbody_.YVelocity > 0) {
        this.stateMachine_.State = PlayerState.INITIALLY_FALLING_OFF_LEDGE;
        this.initiallyFallingTimer_ = 3;
      }

      if (this.stateMachine_.State == PlayerState.WALL_SLIDING &&
          this.rigidbody_.YVelocity > 0) {
        this.playerRigidbody_.YAcceleration =
            PlayerConstants.WALL_SLIDING_GRAVITY;
      } else {
        this.playerRigidbody_.YAcceleration = PlayerConstants.GRAVITY;
      }
    }

    [OnTick]
    private void TickAnimation_(TickAnimationEvent _) {
      var useStateColor = false;
      if (useStateColor) {
        var stateColor = this.stateMachine_.State switch {
            PlayerState.STANDING                    => Color.White,
            PlayerState.WALKING                     => Color.Yellow,
            PlayerState.RUNNING                     => Color.Orange,
            PlayerState.TURNING                     => Color.Magenta,
            PlayerState.STOPPING                    => Color.Bisque,
            PlayerState.DUCKING                     => Color.Chartreuse,
            PlayerState.DUCKWALKING                 => Color.ForestGreen,
            PlayerState.SLIDING                     => Color.LightGreen,
            PlayerState.JUMPING                     => Color.Cyan,
            PlayerState.WALL_SLIDING                => Color.MediumPurple,
            PlayerState.WALLJUMPING                 => Color.Maroon,
            PlayerState.BACKFLIPPING                => Color.Purple,
            PlayerState.LONGJUMPING                 => Color.DodgerBlue,
            PlayerState.FALLING                     => Color.RoyalBlue,
            PlayerState.LANDING                     => Color.Blue,
            PlayerState.INITIALLY_FALLING_OFF_LEDGE => Color.CadetBlue,
            _                                       => Color.Black,
        };
        this.boxPlayerRenderer_.Color = stateColor;
      } else {
        this.boxPlayerRenderer_.Color = ColorConstants.WHITE;
      }
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