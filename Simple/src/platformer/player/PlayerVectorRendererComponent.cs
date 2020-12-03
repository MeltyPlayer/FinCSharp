using System;
using System.Collections.Generic;

using fin.app;
using fin.app.events;
using fin.app.node;
using fin.graphics.camera;
using fin.graphics.color;
using fin.math;
using fin.math.number;
using fin.skeleton;

namespace simple.platformer.player {
  public class PlayerVectorRendererComponent : IComponent {
    private readonly PlayerRigidbody playerRigidbody_;
    private readonly PlayerStateMachine stateMachine_;

    // TODO: Pull out animation logic.
    private readonly IBone2 root_;

    private readonly IBone2 hipCenter_, hipLeft_, hipRight_;
    private readonly IBone2 upperLegLeft_, upperLegRight_;
    private readonly IBone2 lowerLegLeft_, lowerLegRight_;

    private float legHeight_ = 0;
    private float xDir_ = 1;

    public PlayerVectorRendererComponent(
        PlayerRigidbody playerRigidbody,
        PlayerStateMachine stateMachine) {
      this.playerRigidbody_ = playerRigidbody;
      this.stateMachine_ = stateMachine;

      this.root_ = Bone2.NewRoot(90);

      var playerHeight = PlayerConstants.VSIZE;

      var hipWidth = PlayerConstants.HSIZE * .7f;
      this.hipCenter_ = this.root_.AddChild(0, 0);
      this.hipLeft_ = this.hipCenter_.AddChild(hipWidth / 2, 90);
      this.hipRight_ = this.hipCenter_.AddChild(hipWidth / 2, -90);

      var legHeight = playerHeight / 2;

      var upperLegLength = legHeight / 2;
      this.upperLegLeft_ = this.hipLeft_.AddChild(upperLegLength, 90);
      this.upperLegRight_ = this.hipRight_.AddChild(upperLegLength, -90);

      var lowerLegLength = legHeight - upperLegLength;
      this.lowerLegLeft_ = this.upperLegLeft_.AddChild(lowerLegLength, 0);
      this.lowerLegRight_ = this.upperLegRight_.AddChild(lowerLegLength, 0);
    }

    private IRangedFloat frameFraction_ = new CircularFraction(0);

    [OnTick]
    private void TickAnimation_(TickAnimationEvent _) {
      var xVel = this.playerRigidbody_.XVelocity;

      var xVelSign = MathF.Sign(xVel);
      if (xVelSign != 0) {
        this.xDir_ = xVelSign;
      }

      var isStanding = this.stateMachine_.State == PlayerState.STANDING;
      var isWalking = this.stateMachine_.State == PlayerState.WALKING;
      var isRealRunning = this.stateMachine_.State == PlayerState.RUNNING;

      if (isStanding) {
        this.frameFraction_.Value += .01f;
      }

      if (isWalking) {
        var walkFraction = FloatMath.Abs(this.playerRigidbody_.XVelocity) /
                           PlayerConstants.UPRIGHT_MAX_SLOW_XSPD;
        var animationSpeed = FloatMath.Max(.01f, .02f * walkFraction);

        this.frameFraction_.Value += animationSpeed;
      }

      if (isRealRunning) {
        var runFraction = FloatMath.Abs(this.playerRigidbody_.XVelocity) /
                          PlayerConstants.UPRIGHT_MAX_FAST_XSPD;
        var animationSpeed = FloatMath.Max(.01f, .04f * runFraction);

        this.frameFraction_.Value += animationSpeed;
      }

      var frameFraction = this.frameFraction_.Value;
      var frameAngle = this.frameFraction_.Value * 360;

      var hipWidth = PlayerConstants.HSIZE * .6f;
      var backHipWidth = .4f * hipWidth;
      var frontHipWidth = hipWidth - backHipWidth;

      if (isStanding) {
        this.hipCenter_.Transform.RelativeDeg = 0;
        this.hipLeft_.Transform.Length = frontHipWidth;
        this.hipRight_.Transform.Length = backHipWidth;

        var leanAngle = this.OscillateAround_(15, 15, frameAngle);

        this.upperLegLeft_.Transform.RelativeDeg = 90 + leanAngle;
        this.upperLegRight_.Transform.RelativeDeg = -90 + leanAngle;

        this.lowerLegLeft_.Transform.RelativeDeg = -leanAngle;
        this.lowerLegRight_.Transform.RelativeDeg = -leanAngle;
      }


      if (isWalking) {
        var hipAngle = TrigMath.LenDegX(15, -20 + frameAngle);
        this.hipCenter_.Transform.RelativeDeg = hipAngle;

        this.hipLeft_.Transform.Length =
            frontHipWidth * this.OscillateAround_(1, .5f, hipAngle + 180);
        this.hipRight_.Transform.Length =
            backHipWidth * this.OscillateAround_(1, .5f, hipAngle);

        var upperLegRange = 20;
        this.upperLegLeft_.Transform.RelativeDeg =
            90 + TrigMath.LenDegX(upperLegRange/2, frameAngle);
        this.upperLegRight_.Transform.RelativeDeg =
            -90 + TrigMath.LenDegX(upperLegRange/2, frameAngle + 180);

        var lowerLegAngle = -15 + frameAngle;
        var lowerLegRange = 30;
        this.lowerLegLeft_.Transform.RelativeDeg =
            -lowerLegRange / 2 +
            TrigMath.LenDegX(lowerLegRange / 2, lowerLegAngle);
        this.lowerLegRight_.Transform.RelativeDeg =
            -lowerLegRange / 2 +
            TrigMath.LenDegX(lowerLegRange / 2, lowerLegAngle + 180);
      }


      if (isRealRunning) {
        var hipAngle = TrigMath.LenDegX(15, -45 + frameAngle);
        this.hipCenter_.Transform.RelativeDeg = hipAngle;

        this.hipLeft_.Transform.Length =
            frontHipWidth * this.OscillateAround_(1, .5f, hipAngle + 180);
        this.hipRight_.Transform.Length =
            backHipWidth * this.OscillateAround_(1, .5f, hipAngle);

        this.upperLegLeft_.Transform.RelativeDeg =
            180 + this.CalcUpperBoneAngle_(frameFraction, false);
        this.upperLegRight_.Transform.RelativeDeg =
            this.CalcUpperBoneAngle_(frameFraction, true);

        this.lowerLegLeft_.Transform.RelativeDeg =
            this.CalcLowerBoneAngle_(frameFraction, false);
        this.lowerLegRight_.Transform.RelativeDeg =
            this.CalcLowerBoneAngle_(frameFraction, true);
      }

      // TODO: Should this happen automatically?
      this.ForEachBone_(bone => bone.UpdateMatrices());

      var leftHeight =
          FloatMath.Abs(TrigMath.LenDegY(this.upperLegLeft_.Transform.Length,
                                         this.upperLegLeft_.Transform
                                             .GlobalDeg)) +
          FloatMath.Abs(TrigMath.LenDegY(this.lowerLegLeft_.Transform.Length,
                                         this.lowerLegLeft_.Transform
                                             .GlobalDeg));
      var rightHeight =
          FloatMath.Abs(TrigMath.LenDegY(this.upperLegRight_.Transform.Length,
                                         this.upperLegRight_.Transform
                                             .GlobalDeg)) +
          FloatMath.Abs(TrigMath.LenDegY(this.lowerLegRight_.Transform.Length,
                                         this.lowerLegRight_.Transform
                                             .GlobalDeg));
      this.legHeight_ = FloatMath.Max(leftHeight, rightHeight);
    }

    [OnTick]
    private void RenderForOrthographicCamera_(
        RenderForOrthographicCameraTickEvent evt) {
      var g = evt.Graphics;
      var r2d = g.Render2d;
      var t = g.Transform;

      g.Primitives.VertexColor(ColorConstants.WHITE);

      t.Push()
       .Translate(this.playerRigidbody_.CenterX,
                  this.playerRigidbody_.BottomY - this.legHeight_)
       .Scale(this.xDir_, 1);

      this.ForEachBone_(bone => {
        var transform = bone.Transform;
        r2d.Circle(transform.X, transform.Y, 3, 10, true);

        r2d.Line(bone.Start, bone.End);
      });

      t.Pop();
    }

    private float CalcUpperBoneAngle_(float frac, bool opposite) {
      if (opposite) {
        frac = (frac + .5f) % 1;
      }

      return -20 - 95 * MathF.Pow(MathF.Sin(frac * MathF.PI), 2);
    }

    private float CalcLowerBoneAngle_(float frac, bool opposite) {
      if (opposite) {
        frac = (frac + .5f) % 1;
      }

      if (frac < .66) {
        return -70 + 50 * MathF.Sin(2 * MathF.PI * frac * .75f);
      }

      return -70 + 50 * MathF.Sin(MathF.PI * (1 + 3 * (frac - .66f)));
    }


    private readonly Queue<IBone2> boneQueue_ = new Queue<IBone2>();

    private void ForEachBone_(Action<IBone2> handler) {
      /*this.boneQueue_.Enqueue(this.root_);
      while (this.boneQueue_.TryDequeue(out var bone)) {
        handler(bone);

        var children = bone.Children;
        foreach (var child in children) {
          this.boneQueue_.Enqueue(child);
        }
      }*/

      this.ForBoneAndChildren_(this.root_, handler);
    }

    private void ForBoneAndChildren_(IBone2 bone, Action<IBone2> handler) {
      handler(bone);

      foreach (var child in bone.Children) {
        this.ForBoneAndChildren_(child, handler);
      }
    }

    private float OscillateAround_(
        float value,
        float amplitude,
        float degrees) =>
        value - amplitude / 2 + amplitude * MathF.Sin(degrees / 180 * MathF.PI);
  }
}