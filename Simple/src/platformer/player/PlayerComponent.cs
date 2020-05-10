using fin.app;
using fin.app.events;
using fin.app.node;
using fin.graphics.camera;
using fin.graphics.color;
using fin.input;

using CMath = System.Math;
using Math = fin.math.Math;

namespace simple.platformer.player {
  public class PlayerComponent : BComponent {
    private IGamepad gamepad_;

    private readonly PlayerStateMachine stateMachine_ = new PlayerStateMachine {
        State = PlayerState.STANDING,
    };

    private readonly Rigidbody rigidbody_;
    private readonly BoxPlayerRenderer boxPlayerRenderer_;

    private const double SIZE = 32;

    private const double GROUND_REACTION_FRAC = .2;
    private const double GROUND_SLOW_XACC = .4;
    private const double GROUND_FAST_XACC = .6;
    private const double GROUND_FRICTION = .2;

    private const double AIR_SLOW_XACC = .3;
    private const double AIR_FAST_XACC = .4;
    private const double AIR_FRICTION = .1;

    private const double MAX_SLOW_XSPD = 2.5;
    private const double MAX_FAST_XSPD = 5;

    private const double FLOOR_Y = 64;
    private const double GRAVITY = -.7;

    private const double JUMP_HEIGHT = PlayerComponent.SIZE * 3;
    private readonly double jumpSpeed_ = PlayerComponent.CalculateJumpSpeed_(PlayerComponent.JUMP_HEIGHT);

    private const double BACKFLIP_JUMP_HEIGHT = PlayerComponent.SIZE * 4;

    private readonly double backflipJumpSpeed_ =
        PlayerComponent.CalculateJumpSpeed_(PlayerComponent.BACKFLIP_JUMP_HEIGHT);

    private static double CalculateJumpSpeed_(double height) =>
        CMath.Sqrt(-2 * PlayerComponent.GRAVITY * height);

    public PlayerComponent(IGamepad gamepad) {
      this.gamepad_ = gamepad;

      this.rigidbody_ = new Rigidbody {
          Y = PlayerComponent.FLOOR_Y,
          YAcceleration = PlayerComponent.GRAVITY,
          MaxYSpeed = double.MaxValue,
      };

      this.boxPlayerRenderer_ = new BoxPlayerRenderer {
          Rigidbody = this.rigidbody_,
          Size = PlayerComponent.SIZE,
      };
    }

    protected override void Discard() {}

    [OnTick]
    private void Control_(ControlEvent _) {
      var primaryAnalogStick = this.gamepad_[AnalogStickType.PRIMARY];
      var axes = primaryAnalogStick.NormalizedAxes;

      var xAxis = axes.X;

      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      var isRunning = runButton.IsDown;

      if (this.stateMachine_.CanMoveOnGround) {
        var groundAcceleration =
            isRunning ? PlayerComponent.GROUND_FAST_XACC : PlayerComponent.GROUND_SLOW_XACC;
        var reactionFraction =
            Math.Sign(xAxis) == -Math.Sign(this.rigidbody_.XVelocity)
                ? PlayerComponent.GROUND_REACTION_FRAC
                : 1;

        this.rigidbody_.XAcceleration =
            groundAcceleration * reactionFraction * xAxis;

        // If holding a direction on the ground, we're either turning, running, or walking.
        if (Math.Sign(xAxis) != 0) {
          this.stateMachine_.State = reactionFraction != 1
                                         ? PlayerState.TURNING
                                         : isRunning
                                             ? PlayerState.RUNNING
                                             : PlayerState.WALKING;
        }
        // If not holding a direction on the ground but velocity is not zero, we're stopping.
        else if (Math.Abs(this.rigidbody_.XVelocity) > .001) {
          this.stateMachine_.State = PlayerState.STOPPING;
        }
      }

      if (this.stateMachine_.CanMoveInAir) {
        var airAcceleration =
            isRunning ? PlayerComponent.AIR_FAST_XACC : PlayerComponent.AIR_SLOW_XACC;
        this.rigidbody_.XAcceleration = airAcceleration * xAxis;
      }

      var jumpButton = this.gamepad_[FaceButtonType.PRIMARY];
      if (this.stateMachine_.IsOnGround) {
        if (jumpButton.IsPressed) {
          var isBackflip = this.stateMachine_.State == PlayerState.TURNING &&
                           Math.Abs(this.rigidbody_.XVelocity) > PlayerComponent.MAX_SLOW_XSPD;
          var newYVel =
              isBackflip ? this.backflipJumpSpeed_ : this.jumpSpeed_;
          var newState =
              isBackflip ? PlayerState.BACKFLIPPING : PlayerState.JUMPING;

          this.rigidbody_.YVelocity = newYVel;
          this.stateMachine_.State = newState;

          if (isBackflip) {
            this.rigidbody_.XVelocity = 0;
          }
        }
      }
      else if (this.stateMachine_.IsJumping) {
        if (jumpButton.IsReleased) {
          this.stateMachine_.State = PlayerState.FALLING;
          //this.yVel_ = GRAVITY / 10;
          this.rigidbody_.YVelocity /= 2;
        }
      }
    }

    [OnTick]
    private void Physics_(PhysicsEvent _) {
      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      var isRunning = runButton.IsDown;
      this.rigidbody_.MaxXSpeed = isRunning ? PlayerComponent.MAX_FAST_XSPD : PlayerComponent.MAX_SLOW_XSPD;

      var isOnGround = this.stateMachine_.IsOnGround;
      this.rigidbody_.Friction = isOnGround ? PlayerComponent.GROUND_FRICTION : PlayerComponent.AIR_FRICTION;

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
    private void Collision_(CollisionEvent _) {
      if (this.rigidbody_.Y <= PlayerComponent.FLOOR_Y) {
        if (this.stateMachine_.IsInAir) {
          this.stateMachine_.State = PlayerState.STANDING;
        }
        this.rigidbody_.Y = PlayerComponent.FLOOR_Y;
        this.rigidbody_.YVelocity = 0;
      }

      // If falling while meant to be on the ground, then switch to falling state.
      if (this.stateMachine_.IsOnGround && this.rigidbody_.YVelocity < 0) {
        this.stateMachine_.State = PlayerState.FALLING;
      }
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      evt.Graphics.Primitives.VertexColor(ColorConstants.GREEN);
      evt.Graphics.Render2d.Rectangle(0,
                                      (int) (480 - PlayerComponent.FLOOR_Y),
                                      640,
                                      (int) PlayerComponent.FLOOR_Y,
                                      true);

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
      this.boxPlayerRenderer_.Render(evt.Graphics);
    }
  }
}