using fin.app;
using fin.app.events;
using fin.app.node;
using fin.graphics;
using fin.graphics.camera;
using fin.graphics.color;
using fin.input;
using fin.math;

using simple.platformer.world;

namespace simple.platformer.player {
  public class PlayerComponent : BComponent {
    private IGamepad gamepad_;

    private readonly PlayerStateMachine stateMachine_ = new PlayerStateMachine {
        State = PlayerState.STANDING,
    };

    private readonly Rigidbody rigidbody_;
    private readonly PlayerMotor motor_;
    private readonly PlayerCollider collider_;
    private readonly BoxPlayerRenderer boxPlayerRenderer_;

    private double prevHeldY_ = 0;

    public PlayerComponent(IGamepad gamepad) {
      this.gamepad_ = gamepad;

      this.rigidbody_ = new Rigidbody {
          Position = (LevelConstants.SIZE * 10, LevelConstants.SIZE * 13),
          YAcceleration = PlayerConstants.GRAVITY,
          MaxYSpeed = double.MaxValue,
      };

      this.motor_ = new PlayerMotor {
          StateMachine = this.stateMachine_,
          Rigidbody = this.rigidbody_,
      };

      this.collider_ = new PlayerCollider {
          StateMachine = this.stateMachine_,
          Rigidbody = this.rigidbody_,
      };

      this.boxPlayerRenderer_ = new BoxPlayerRenderer {
          Rigidbody = this.rigidbody_,
          HSize = PlayerConstants.HSIZE,
          VSize = PlayerConstants.VSIZE,
      };
    }

    protected override void Discard() {}

    [OnTick]
    private void ProcessInputs_(ProcessInputsEvent _) {
      var primaryAnalogStick = this.gamepad_[AnalogStickType.PRIMARY];
      var heldAxes = primaryAnalogStick.NormalizedAxes;
      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      this.motor_.ScheduleMoveAttempt(heldAxes.X, runButton.IsDown);

      if (Math.IsIncreasing(heldAxes.Y, -.5, this.prevHeldY_)) {
        this.motor_.ScheduleDuckStartAttempt();
      }
      if (Math.IsIncreasing(this.prevHeldY_, -.5, heldAxes.Y)) {
        this.motor_.ScheduleDuckStopAttempt();
      }
      this.prevHeldY_ = heldAxes.Y;

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
      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      var isRunning = runButton.IsDown;
      this.rigidbody_.MaxXSpeed = isRunning
                                      ? PlayerConstants.MAX_FAST_XSPD
                                      : PlayerConstants.MAX_SLOW_XSPD;

      if (this.stateMachine_.IsOnGround) {
        if (this.stateMachine_.State != PlayerState.SLIDING) {
          this.rigidbody_.Friction = PlayerConstants.GROUND_FRICTION;
        }
        else {
          this.rigidbody_.Friction = PlayerConstants.GROUND_SLIDING_FRICTION;
        }
      }
      else {
        this.rigidbody_.Friction = PlayerConstants.AIR_FRICTION;
      }

      this.rigidbody_.TickPhysics(3);

      var (xVelocity, yVelocity) = this.rigidbody_.Velocity;

      // When comes to a stop, start standing.
      if (this.stateMachine_.State == PlayerState.STOPPING && xVelocity == 0) {
        this.stateMachine_.State = PlayerState.STANDING;
      }

      // When transitions to negative y velocity in air after a jump, start falling.
      if (this.stateMachine_.IsMovingUpwardInAir && yVelocity < 0) {
        this.stateMachine_.State = PlayerState.FALLING;
      }
    }

    [OnTick]
    private void TickCollisions_(TickCollisionsEvent _) {
      this.collider_.TickCollisions();

      // If falling while meant to be on the ground, then switch to falling state.
      if (this.stateMachine_.IsOnGround && this.rigidbody_.YVelocity < 0) {
        this.stateMachine_.State = PlayerState.FALLING;
      }
    }

    [OnTick]
    private void TickAnimation_(TickAnimationEvent _) {
      this.boxPlayerRenderer_.Color = this.stateMachine_.State switch {
          PlayerState.STANDING     => ColorConstants.WHITE,
          PlayerState.WALKING      => ColorConstants.YELLOW,
          PlayerState.RUNNING      => ColorConstants.ORANGE,
          PlayerState.TURNING      => ColorConstants.MAGENTA,
          PlayerState.STOPPING     => ColorConstants.RED,
          PlayerState.DUCKING      => ColorConstants.TEAL,
          PlayerState.SLIDING      => ColorConstants.GREEN,
          PlayerState.JUMPING      => ColorConstants.BLUE,
          PlayerState.BACKFLIPPING => ColorConstants.PURPLE,
          PlayerState.FALLING      => ColorConstants.CYAN,
          _                        => ColorConstants.BLACK,
      };
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      var level = LevelConstants.LEVEL;
      var size = LevelConstants.SIZE;

      var primitives = evt.Graphics.Primitives;
      primitives.VertexColor(ColorConstants.WHITE).Begin(PrimitiveType.QUADS);
      foreach (var node in level) {
        var c = node.X;
        var r = node.Y;

        var (x, y) = (size * c, size * r);

        var leftX = (int) x;
        var rightX = (int) (x + size);
        var topY = (int) y;
        var bottomY = (int) (y + size);

        primitives.Vertex(leftX, topY)
                  .Vertex(rightX, topY)
                  .Vertex(rightX, bottomY)
                  .Vertex(leftX, bottomY);
      }
      primitives.End();

      var isWide = this.stateMachine_.State == PlayerState.SLIDING;
      var isTall = this.stateMachine_.State != PlayerState.DUCKING &&
                   this.stateMachine_.State != PlayerState.SLIDING;
      this.boxPlayerRenderer_.HSize =
          !isWide ? PlayerConstants.HSIZE : PlayerConstants.VSIZE;
      this.boxPlayerRenderer_.VSize =
          isTall ? PlayerConstants.VSIZE : PlayerConstants.HSIZE;
      this.boxPlayerRenderer_.Render(evt.Graphics);
    }
  }
}