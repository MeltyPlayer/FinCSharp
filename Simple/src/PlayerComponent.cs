using System.Collections.Generic;

using fin.app;
using fin.app.events;
using fin.app.node;
using fin.graphics.camera;
using fin.graphics.color;
using fin.input;
using fin.math;

using CMath = System.Math;

namespace simple {
  public enum PlayerState {
    STANDING,
    WALKING,
    RUNNING,
    TURNING,
    STOPPING,

    JUMPING,
    FALLING,
  }

  public class PlayerComponent : BComponent {
    private IGamepad gamepad_;

    private const double SIZE = 32;

    private const double GROUND_REACTION_FRAC = .2;
    private const double GROUND_SLOW_XACC = .5;
    private const double GROUND_FAST_XACC = .6;
    private const double GROUND_FRICTION = .2;

    private const double AIR_SLOW_XACC = .3;
    private const double AIR_FAST_XACC = .4;

    private const double MAX_SLOW_XSPD = 3;
    private const double MAX_FAST_XSPD = 5;

    private const double FLOOR_Y = 10;
    private const double JUMP_HEIGHT = SIZE * 3;
    private const double GRAVITY = -.7;

    private List<PlayerState> groundStates_ = new List<PlayerState>(new[] {
        PlayerState.STANDING, PlayerState.WALKING, PlayerState.RUNNING,
        PlayerState.TURNING, PlayerState.STOPPING,
    });

    private List<PlayerState> canMoveGroundStates_ = new List<PlayerState>(
        new[] {
            PlayerState.STANDING, PlayerState.WALKING, PlayerState.RUNNING,
            PlayerState.TURNING, PlayerState.STOPPING,
        });

    private List<PlayerState> airStates_ = new List<PlayerState>(
        new[] {
            PlayerState.JUMPING, PlayerState.FALLING,
        });

    private List<PlayerState> canMoveAirStates_ = new List<PlayerState>(
        new[] {
            PlayerState.JUMPING, PlayerState.FALLING,
        });

    private readonly double jumpSpeed_ =
        CMath.Sqrt(-2 * GRAVITY * JUMP_HEIGHT);

    private double x_ = 0;
    private double y_ = FLOOR_Y;

    private double xVel_ = 0;
    private double yVel_ = 0;

    private double xAcc_ = 0;
    private double yAcc_ = GRAVITY;

    private PlayerState state_ = PlayerState.STANDING;

    public PlayerComponent(IGamepad gamepad) {
      this.gamepad_ = gamepad;
    }

    protected override void Discard() {}

    [OnTick]
    private void Control_(ControlEvent _) {
      var primaryAnalogStick = this.gamepad_[AnalogStickType.PRIMARY];
      var axes = primaryAnalogStick.NormalizedAxes;

      var xAxis = axes.X;

      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      var isRunning = runButton.IsDown;

      var canMoveOnGround = this.canMoveGroundStates_.Contains(this.state_);
      if (canMoveOnGround) {
        var groundAcceleration =
            isRunning ? GROUND_FAST_XACC : GROUND_SLOW_XACC;
        var reactionFraction = Math.Sign(xAxis) == -Math.Sign(this.xVel_)
                                   ? PlayerComponent.GROUND_REACTION_FRAC
                                   : 1;

        this.xAcc_ = groundAcceleration * reactionFraction * xAxis;

        // If holding a direction on the ground, we're either turning, running, or walking.
        if (Math.Sign(xAxis) != 0) {
          this.state_ = reactionFraction != 1 ? PlayerState.TURNING :
                        isRunning ? PlayerState.RUNNING : PlayerState.WALKING;
        }
        else {
          // If not holding a direction on the ground but velocity is not zero, we're stopping.
          if (Math.Abs(this.xVel_) > .001) {
            this.state_ = PlayerState.STOPPING;
          }
        }
      }

      var canMoveInAir = this.canMoveAirStates_.Contains(this.state_);
      if (canMoveInAir) {
        var airAcceleration =
            isRunning ? AIR_FAST_XACC : AIR_SLOW_XACC;
        this.xAcc_ = airAcceleration * xAxis;
      }

      var jumpButton = this.gamepad_[FaceButtonType.PRIMARY];
      if (this.IsOnGround) {
        if (jumpButton.IsPressed) {
          this.yVel_ = this.jumpSpeed_;
          this.state_ = PlayerState.JUMPING;
        }
      }
      else if (this.state_ == PlayerState.JUMPING) {
        if (jumpButton.IsReleased) {
          this.state_ = PlayerState.FALLING;
          this.yVel_ = GRAVITY / 10;
        }
      }
    }

    [OnTick]
    private void Physics_(PhysicsEvent _) {
      var runButton = this.gamepad_[FaceButtonType.SECONDARY];
      var isRunning = runButton.IsDown;

      var maxXSpeed = isRunning
                          ? MAX_FAST_XSPD
                          : MAX_SLOW_XSPD;
      var isOnGround = this.IsOnGround;

      int n = 3;
      double eulerFrac = 1d / n;
      for (int i = 0; i < n; ++i) {
        if (isOnGround) {
          this.xVel_ =
              Math.AddTowards(this.xVel_, 0, GROUND_FRICTION * eulerFrac);
        }
        this.xVel_ = Math.Clamp(-maxXSpeed,
                                this.xVel_ + this.xAcc_ * eulerFrac,
                                maxXSpeed);
        this.x_ += this.xVel_ * eulerFrac;

        this.yVel_ += this.yAcc_ * eulerFrac;
        this.y_ += this.yVel_ * eulerFrac;
      }

      // When comes to a stop, start standing.
      if (this.state_ == PlayerState.STOPPING && this.xVel_ == 0) {
        this.state_ = PlayerState.STANDING;
      }

      // When transitions to negative y velocity in air after a jump, start falling.
      if (this.state_ == PlayerState.JUMPING && this.yVel_ < 0) {
        this.state_ = PlayerState.FALLING;
      }
    }

    private bool IsInAir => this.airStates_.Contains(this.state_);
    private bool IsOnGround => this.groundStates_.Contains(this.state_);

    [OnTick]
    private void Collision_(CollisionEvent _) {
      if (this.y_ <= FLOOR_Y) {
        if (this.IsInAir) {
          this.state_ = PlayerState.STANDING;
        }
        this.y_ = FLOOR_Y;
        this.yVel_ = 0;
      }

      // If falling while meant to be on the ground, then switch to falling state.
      if (this.IsOnGround && this.yVel_ < 0) {
        this.state_ = PlayerState.FALLING;
      }
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      evt.Graphics.Primitives.VertexColor(ColorConstants.GREEN);
      evt.Graphics.Render2d.Rectangle(0,
                                      (int) (480 - FLOOR_Y),
                                      640,
                                      (int) FLOOR_Y,
                                      true);

      var playerColor = this.state_ switch {
          PlayerState.STANDING => ColorConstants.WHITE,
          PlayerState.WALKING  => ColorConstants.YELLOW,
          PlayerState.RUNNING  => ColorConstants.ORANGE,
          PlayerState.TURNING  => ColorConstants.MAGENTA,
          PlayerState.STOPPING => ColorConstants.RED,
          PlayerState.JUMPING  => ColorConstants.BLUE,
          PlayerState.FALLING  => ColorConstants.CYAN,
          _                    => ColorConstants.BLACK,
      };

      evt.Graphics.Primitives.VertexColor(playerColor);
      evt.Graphics.Render2d.Rectangle((int) (this.x_ - SIZE / 2),
                                      (int) (480 - (this.y_ + SIZE)),
                                      (int) SIZE,
                                      (int) SIZE,
                                      false);
    }
  }
}