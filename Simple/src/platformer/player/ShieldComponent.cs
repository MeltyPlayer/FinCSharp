using System;

using fin.app;
using fin.graphics.camera;
using fin.input;
using fin.input.gamepad;
using fin.math;

namespace simple.platformer.player {
  public class ShieldComponent : IItemComponent {
    private readonly IGamepad gamepad_;
    private readonly PlayerRigidbody playerRigidbody_;
    private readonly PlayerStateMachine stateMachine_;

    private bool isShieldOut_ = false;
    private float handDis_ = 0;
    private float handDeg_ = 0;

    public ShieldComponent(
        IGamepad gamepad,
        PlayerRigidbody playerRigidbody,
        PlayerStateMachine stateMachine) {
      this.gamepad_ = gamepad;
      this.playerRigidbody_ = playerRigidbody;
      this.stateMachine_ = stateMachine;
    }

    public void ProcessInputs(ProcessInputsEvent _) {
      var secondaryAnalogStick = this.gamepad_[AnalogStickType.SECONDARY];
      var secStickX = secondaryAnalogStick.RawAxes.X;
      var secStickY = secondaryAnalogStick.RawAxes.Y;

      var secStickMag = TrigMath.DistanceBetween(0, 0, secStickX, secStickY);
      if (secStickMag < GamepadConstants.DEADZONE) {
        secStickMag = 0;
      }

      var wasShieldOut = this.isShieldOut_;
      this.isShieldOut_ = secStickMag > 0;

      if (this.isShieldOut_) {
        var secStickDeg = TrigMath.DegreesBetween(0, 0, secStickX, secStickY);

        var toHandDeg = secStickDeg;

        var maxHorizontalHandDis = this.playerRigidbody_.Width / 2 + 2;
        var maxVerticalHandDis = this.playerRigidbody_.Height / 2 + 2;
        var maxHandDis =
            FloatMath.Abs(
                TrigMath.LenDegX(maxHorizontalHandDis, secStickDeg)) +
            FloatMath.Abs(
                TrigMath.LenDegY(maxVerticalHandDis, secStickDeg));

        var toHandDis = maxHandDis * secStickMag;

        if (!wasShieldOut) {
          this.handDis_ = toHandDis;
          this.handDeg_ = toHandDeg;
        } else {
          var accFactor = 3;

          this.handDis_ += (toHandDis - this.handDis_) / accFactor;
          this.handDeg_ +=
              TrigMath.DifferenceInDegrees(toHandDeg, this.handDeg_) /
              accFactor;
        }
      }
    }

    public void TickCollisions(TickCollisionsEvent _) {
      // TODO: Any collisions?
    }

    public void RenderForOrthographicCamera(
        RenderForOrthographicCameraTickEvent evt) {
      if (!this.isShieldOut_) {
        return;
      }

      var g = evt.Graphics;

      var t = g.Transform;
      var (x, y) = (this.playerRigidbody_.CenterX,
                    this.playerRigidbody_.CenterY);

      var shieldWidth = 32;
      var shieldDepth = 4;
      t.Push()
       .Translate(x, y)
       .Rotate(this.handDeg_)
       .Translate(this.handDis_, 0);
      g.Render2d.Rectangle(-shieldDepth * .5f,
                           -shieldWidth * .5f,
                           shieldDepth,
                           shieldWidth,
                           true);
      t.Pop();
    }
  }
}