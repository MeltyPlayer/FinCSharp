using System;

using fin.app;
using fin.graphics.camera;
using fin.input;
using fin.input.gamepad;
using fin.math;
using fin.math.geometry;

using simple.platformer.player.sfx;

namespace simple.platformer.player {
  public class WhipComponent : IItemComponent {
    private readonly IGamepad gamepad_;
    private readonly PlayerRigidbody playerRigidbody_;
    private readonly PlayerSoundsComponent playerSounds_;
    private readonly PlayerStateMachine stateMachine_;

    private readonly float whipLength_ = 128;

    private MutableVector2<float>[] whipPoints_;

    private bool isWhipping_ = false;
    private float whipTimeFraction_ = 0;

    private bool isStoring_ = false;
    private float storedWhipTimer_ = -1;

    private float whipStrength_ = 0;
    private float whipDeg_ = 0;

    public WhipComponent(
        IGamepad gamepad,
        PlayerRigidbody playerRigidbody,
        PlayerSoundsComponent playerSounds,
        PlayerStateMachine stateMachine) {
      this.gamepad_ = gamepad;
      this.playerRigidbody_ = playerRigidbody;
      this.playerSounds_ = playerSounds;
      this.stateMachine_ = stateMachine;

      var whipPointCount = 10;
      this.whipPoints_ = new MutableVector2<float>[whipPointCount];
      for (var i = 0; i < whipPointCount; ++i) {
        this.whipPoints_[i] = new MutableVector2<float>();
      }
    }

    public void ProcessInputs(ProcessInputsEvent _) {
      if (this.whipTimeFraction_ >= 1) {
        this.isWhipping_ = false;
      }
      if (this.isWhipping_) {
        this.whipTimeFraction_ = MathF.Min(this.whipTimeFraction_ + .1f, 1);
        return;
      }

      var secondaryAnalogStick = this.gamepad_[AnalogStickType.SECONDARY];
      var secStickX = secondaryAnalogStick.RawAxes.X;
      var secStickY = secondaryAnalogStick.RawAxes.Y;

      var secStickMag = TrigMath.DistanceBetween(0, 0, secStickX, secStickY);
      if (secStickMag < GamepadConstants.DEADZONE) {
        secStickMag = 0;
      }

      this.isStoring_ = secStickMag > .8f;
      if (this.isStoring_) {
        var secStickDeg = TrigMath.DegreesBetween(0, 0, secStickX, secStickY);

        this.storedWhipTimer_ = 5;
        this.whipStrength_ = secStickMag;
        this.whipDeg_ = secStickDeg + 180;
      }

      var wasFlicked = this.storedWhipTimer_-- > 0 && secStickMag == 0;
      if (wasFlicked) {
        this.isStoring_ = false;
        this.storedWhipTimer_ = -1;

        this.isWhipping_ = true;
        this.whipTimeFraction_ = 0;

        // TODO: Move this out.
        this.playerSounds_.PlayWhipSound();
      }
    }

    public void TickCollisions(TickCollisionsEvent _) {
      // TODO: Collisions?
    }

    public void TickAnimations(TickAnimationEvent _) {}

    public void RenderForOrthographicCamera(
        RenderForOrthographicCameraTickEvent evt) {
      var g = evt.Graphics;
      var t = g.Transform;
      var r2d = g.Render2d;

      var (x, y) = (this.playerRigidbody_.CenterX,
                    this.playerRigidbody_.CenterY);

      t.Push()
       .Translate(x, y)
       .Rotate(this.whipDeg_);

      // TODO: Track these via a state machine instead.
      if (this.isWhipping_) {
        var maxWhipLen = this.whipLength_ * this.whipStrength_;

        var whipFractionRads =
            MathF.Pow(this.whipTimeFraction_, .5f) * .8f * MathF.PI;
        var whipFraction = MathF.Sin(whipFractionRads);

        var whipLength = maxWhipLen * whipFraction;

        var whipPointCount = this.whipPoints_.Length;
        for (var i = 0; i < whipPointCount; ++i) {
          var whipPoint = this.whipPoints_[i];

          var f = (1f * i) / (whipPointCount - 1f);
          var whipPointX = f * whipLength;

          var whipYRads = 5 * whipFraction * MathF.PI + f * MathF.PI;
          var whipPointY = MathF.Sin(f * MathF.PI) *
                           6 *
                           MathF.Sin(whipYRads);

          whipPoint.X = whipPointX;
          whipPoint.Y = whipPointY;
        }

        r2d.Line(this.whipPoints_);
      } else if (this.isStoring_) {
        r2d.Circle(-this.whipStrength_ * 16, 0, 4, 8, true);
      }

      t.Pop();
    }
  }
}