using fin.app;
using fin.app.events;
using fin.app.node;
using fin.graphics.camera;
using fin.graphics.color;
using fin.input;

namespace simple.platformer.player {
  public class PlayerComponent : BComponent {
    private IGamepad gamepad_;

    private readonly PlayerStateMachine stateMachine_ = new PlayerStateMachine {
        State = PlayerState.STANDING,
    };

    private readonly Rigidbody rigidbody_;
    private readonly PlayerMotor playerMotor_;
    private readonly BoxPlayerRenderer boxPlayerRenderer_;

    public PlayerComponent(IGamepad gamepad) {
      this.gamepad_ = gamepad;

      this.rigidbody_ = new Rigidbody {
          Y = PlayerConstants.FLOOR_Y,
          YAcceleration = PlayerConstants.GRAVITY,
          MaxYSpeed = double.MaxValue,
      };

      this.playerMotor_ = new PlayerMotor {
          Rigidbody = this.rigidbody_,
          StateMachine = this.stateMachine_,
      };

      this.boxPlayerRenderer_ = new BoxPlayerRenderer {
          Rigidbody = this.rigidbody_,
          Size = PlayerConstants.SIZE,
      };
    }

    protected override void Discard() {}

    [OnTick]
    private void ProcessInputs_(ProcessInputsEvent _) {
      var primaryAnalogStick = this.gamepad_[AnalogStickType.PRIMARY];
      var heldAxes = primaryAnalogStick.NormalizedAxes;
      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      this.playerMotor_.ScheduleMoveAttempt(heldAxes.X, runButton.IsDown);

      var jumpButton = this.gamepad_[FaceButtonType.PRIMARY];
      if (jumpButton.IsPressed) {
        this.playerMotor_.ScheduleJumpStartAttempt();
      }
      else if(jumpButton.IsReleased) {
        this.playerMotor_.ScheduleJumpStopAttempt();
      }

      this.playerMotor_.ProcessInputs();
    }

    [OnTick]
    private void TickPhysics_(TickPhysicsEvent _) {
      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      var isRunning = runButton.IsDown;
      this.rigidbody_.MaxXSpeed = isRunning
                                      ? PlayerConstants.MAX_FAST_XSPD
                                      : PlayerConstants.MAX_SLOW_XSPD;

      var isOnGround = this.stateMachine_.IsOnGround;
      this.rigidbody_.Friction = isOnGround
                                     ? PlayerConstants.GROUND_FRICTION
                                     : PlayerConstants.AIR_FRICTION;

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
      if (this.rigidbody_.Y <= PlayerConstants.FLOOR_Y) {
        if (this.stateMachine_.IsInAir) {
          this.stateMachine_.State = PlayerState.STANDING;
        }
        this.rigidbody_.Y = PlayerConstants.FLOOR_Y;
        this.rigidbody_.YVelocity = 0;
      }

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
          PlayerState.JUMPING      => ColorConstants.BLUE,
          PlayerState.BACKFLIPPING => ColorConstants.PURPLE,
          PlayerState.FALLING      => ColorConstants.CYAN,
          _                        => ColorConstants.BLACK,
      };
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      evt.Graphics.Primitives.VertexColor(ColorConstants.GREEN);
      evt.Graphics.Render2d.Rectangle(0,
                                      (int) (480 - PlayerConstants.FLOOR_Y),
                                      640,
                                      (int) PlayerConstants.FLOOR_Y,
                                      true);

      this.boxPlayerRenderer_.Render(evt.Graphics);
    }
  }
}