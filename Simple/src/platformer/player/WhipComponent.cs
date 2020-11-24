using System;

using fin.app;
using fin.graphics.camera;
using fin.input;
using fin.input.gamepad;
using fin.math;

namespace simple.platformer.player {
  public class WhipComponent : IItemComponent {
    private readonly IGamepad gamepad_;
    private readonly PlayerRigidbody playerRigidbody_;
    private readonly PlayerStateMachine stateMachine_;

    private readonly float whipLength_ = 128;

    private bool isWhipping_ = false;
    private float whipTimeFraction_ = 0;

    private bool isStoring_ = false;
    private float storedWhipTimer_ = -1;

    private float whipStrength_ = 0;
    private float whipDeg_ = 0;

    public WhipComponent(
        IGamepad gamepad,
        PlayerRigidbody playerRigidbody,
        PlayerStateMachine stateMachine) {
      this.gamepad_ = gamepad;
      this.playerRigidbody_ = playerRigidbody;
      this.stateMachine_ = stateMachine;
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
      }
    }

    public void TickCollisions(TickCollisionsEvent _) {
      // TODO: Collisions?
    }

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
        var whipFraction = MathF.Sin(this.whipTimeFraction_ * .9f * MathF.PI);
        var whipLen = maxWhipLen * whipFraction;
        r2d.Line(0, 0, whipLen, 0);
      } else if (this.isStoring_) {
        r2d.Circle(-this.whipStrength_ * 16, 0, 4, 8, true);
      }

      t.Pop();
    }
  }
}