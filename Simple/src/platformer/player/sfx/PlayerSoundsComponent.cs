using fin.app;
using fin.app.events;
using fin.app.node;
using fin.audio;
using fin.io;
using fin.math;
using fin.math.random;

namespace simple.platformer.player.sfx {
  public class PlayerSoundsComponent : IComponent {
    private readonly IAudioSource source_;

    private readonly IAudioBuffer bumpWallSound_;
    private readonly IAudioBuffer footstepHeavy_;
    private readonly IAudioBuffer footstepLight_;

    private readonly IAudioBuffer jumpSound_;
    private readonly IAudioBuffer walljumpSound_;
    private readonly IAudioBuffer fallSound_;
    private readonly IAudioBuffer landSound_;
    private readonly IAudioBuffer whipSound_;

    private readonly Rigidbody rigidbody_;
    private readonly PlayerStateMachine stateMachine_;

    private readonly float distanceBetweenFootsteps_ = 72;
    private float distanceToNextFootstep_ = 0;

    public PlayerSoundsComponent(
        IAudio audio,
        Rigidbody rigidbody,
        PlayerStateMachine stateMachine) {
      this.source_ = audio.Factory.NewAudioSource(3);

      var sfx = LocalIo.Resources.GetSubpath("sfx/");
      this.bumpWallSound_ = audio.LoadAsBuffer(sfx.GetFile("bump_wall.ogg"));
      this.footstepHeavy_ =
          audio.LoadAsBuffer(sfx.GetFile("footstep_heavy.ogg"));
      this.footstepLight_ =
          audio.LoadAsBuffer(sfx.GetFile("footstep_light.ogg"));
      this.jumpSound_ = audio.LoadAsBuffer(sfx.GetFile("jump.ogg"));
      this.walljumpSound_ = audio.LoadAsBuffer(sfx.GetFile("walljump.ogg"));
      this.fallSound_ = audio.LoadAsBuffer(sfx.GetFile("fall.ogg"));
      this.landSound_ = audio.LoadAsBuffer(sfx.GetFile("land.ogg"));
      this.whipSound_ = audio.LoadAsBuffer(sfx.GetFile("whip.ogg"));

      this.rigidbody_ = rigidbody;
      this.stateMachine_ = stateMachine;

      this.HookIntoStateMachine_(stateMachine);
    }

    private void HookIntoStateMachine_(PlayerStateMachine stateMachine) {
      stateMachine.OnEnter(PlayerState.JUMPING, this.PlayJumpSound);
      stateMachine.OnEnter(PlayerState.WALLJUMPING, this.PlayWalljumpSound);
      stateMachine.OnEnter(PlayerState.INITIALLY_FALLING_OFF_LEDGE,
                           this.PlayFallSound);
      stateMachine.OnEnter(PlayerState.LANDING, this.PlayLandSound);
    }

    [OnTick]
    private void TickAnimations_(TickAnimationEvent _) {
      if (this.stateMachine_.IsMovingUprightOnGround ||
          this.stateMachine_.IsMovingDuckedOnGround) {
        this.distanceToNextFootstep_ -=
            FloatMath.Abs(this.rigidbody_.XVelocity);
      }

      if (this.distanceToNextFootstep_ <= 0) {
        var isUpright = this.stateMachine_.IsMovingUprightOnGround;
        this.PlayAtRandomPitch_(isUpright
                                    ? this.footstepHeavy_
                                    : this.footstepLight_);

        this.distanceToNextFootstep_ += this.distanceBetweenFootsteps_;
      }
    }

    private void PlayAtRandomPitch_(IAudioBuffer sound) {
      var pitch = (float) (.9f + .2f * FinRandom.Double());
      this.source_.Play(sound, false, pitch);
    }

    public void PlayJumpSound() => this.PlayAtRandomPitch_(this.jumpSound_);

    public void PlayWalljumpSound()
      => this.PlayAtRandomPitch_(this.walljumpSound_);

    public void PlayBumpWallSound()
      => this.PlayAtRandomPitch_(this.bumpWallSound_);

    public void PlayFallSound() => this.PlayAtRandomPitch_(this.fallSound_);
    public void PlayLandSound() => this.PlayAtRandomPitch_(this.landSound_);
    public void PlayWhipSound() => this.PlayAtRandomPitch_(this.whipSound_);
  }
}