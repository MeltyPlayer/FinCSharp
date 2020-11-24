using System;

using fin.app;
using fin.graphics.camera;
using fin.input;
using fin.input.gamepad;
using fin.math;

namespace simple.platformer.player {
  public class SwordComponent : IItemComponent {
    private readonly IGamepad gamepad_;
    private readonly PlayerRigidbody playerRigidbody_;
    private readonly PlayerStateMachine stateMachine_;

    private readonly float maxHandDis_ = 16;
    private readonly float bladeLength_ = 64;

    private float handDis_ = 0;
    private float handDeg_ = 0;

    private float swordDeg_ = 0;
    private float swordDevVel_ = 0;

    public SwordComponent(
        IGamepad gamepad,
        PlayerRigidbody playerRigidbody,
        PlayerStateMachine stateMachine) {
      this.gamepad_ = gamepad;
      this.playerRigidbody_ = playerRigidbody;
      this.stateMachine_ = stateMachine;
    }

    public void ProcessInputs(ProcessInputsEvent _) {
      this.ProcessManualInputs_();
    }

    private void ProcessManualInputs_() {
      var secondaryAnalogStick = this.gamepad_[AnalogStickType.SECONDARY];
      var secStickX = secondaryAnalogStick.RawAxes.X;
      var secStickY = secondaryAnalogStick.RawAxes.Y;

      var secStickMag = TrigMath.DistanceBetween(0, 0, secStickX, secStickY);
      if (secStickMag < GamepadConstants.DEADZONE) {
        secStickMag = 0;
      }
      var secStickDeg = TrigMath.DegreesBetween(0, 0, secStickX, secStickY);

      var isResting = secStickMag == 0;

      // TODO: Detect arcs?

      if (isResting) {
        var xVel = this.playerRigidbody_.XVelocity;
        if (xVel != 0) {
          var sign = FloatMath.Sign(xVel);

          this.handDis_ = this.maxHandDis_;

          if (this.stateMachine_.State == PlayerState.WALKING) {
            this.handDeg_ = 90 - sign * (90 + 45);
            this.swordDeg_ = 90 - sign * 45;
          } else if (this.stateMachine_.State == PlayerState.RUNNING) {
            this.handDeg_ = 90 + sign * (90 + 22);
            this.swordDeg_ = 90 + sign * (90 + 22);
          }
        }
        return;
      }

      // Stick moves hands to lift sword. Angle needs to be closer to have
      // more of an effect.
      var diffToHeld =
          TrigMath.DifferenceInDegrees(secStickDeg, this.handDeg_);
      var normalizedDiffToHeld = MathF.Abs(diffToHeld / 180);

      var liftFactor = secStickMag * (1 - normalizedDiffToHeld);

      //this.swordOutDeg_ += diffToHeld * liftFactor;
      this.handDeg_ =
          TrigMath.AddTowardsDegrees(this.handDeg_,
                                     secStickDeg,
                                     3 * liftFactor);
      this.handDeg_ %= 360;
      //this.swordOutDeg_ = secStickDeg;
      this.handDis_ = 16 * secStickMag;

      // TODO: Should be based on velocity instead.
      // If lifting sword, sword angle lags behind.
      this.swordDeg_ = this.handDeg_;
      /*var diffToTop = TrigMath.DifferenceInDegrees(270, this.swordDeg_);
      var diffBetweenSwordAndHeld =
          TrigMath.DifferenceInDegrees(this.swordOutDeg_, this.swordDeg_);
      if (FloatMath.Sign(diffToTop) == FloatMath.Sign(diffBetweenSwordAndHeld)) {
        this.swordDeg_ += diffBetweenSwordAndHeld / 10;
      } else {
        this.swordDeg_ += diffBetweenSwordAndHeld / 2;
      }*/
    }

    public void TickCollisions(TickCollisionsEvent _) {
      // TODO: Do this via collision detection instead.
      // TODO: Clean this up.
      // Keeps sword rested on the ground.
      /*if (this.stateMachine_.IsOnGround) {
        var bladeFromGroundY = -(this.playerRigidbody_.CenterY -
                                 this.playerRigidbody_.BottomY);
        bladeFromGroundY -= MathF.Abs(
            TrigMath.LenDegY(this.handDis_, this.handDeg_));

        var minAngle = MathF.Acos(bladeFromGroundY / this.bladeLength_) /
                       MathF.PI *
                       180;

        var diffToGround = TrigMath.DifferenceInDegrees(this.swordDeg_, 270);
        if (FloatMath.Abs(diffToGround) <= minAngle) {
          this.swordDeg_ = 270 + FloatMath.Sign(diffToGround) * minAngle;
        }
      }*/
    }
    
    public void RenderForOrthographicCamera(
        RenderForOrthographicCameraTickEvent evt) {
      var g = evt.Graphics;

      var t = g.Transform;
      var (x, y) = (this.playerRigidbody_.CenterX,
                    this.playerRigidbody_.CenterY);

      var bladeLength = this.bladeLength_;
      var bladeWidth = 12;
      t.Push()
       .Translate(x, y)
       .Rotate(this.handDeg_)
       .Translate(this.handDis_, 0)
       .Rotate(-this.handDeg_ + this.swordDeg_);
      g.Render2d.Rectangle(0, -bladeWidth * .5f, bladeLength, bladeWidth, true);
      t.Pop();
    }
  }
}