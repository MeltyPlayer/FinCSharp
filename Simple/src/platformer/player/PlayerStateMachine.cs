using System;
using System.Linq;

namespace simple.platformer.player {
  public enum PlayerState {
    STANDING,
    WALKING,
    RUNNING,
    TURNING,
    STOPPING,
    DUCKING,
    DUCKWALKING,
    LANDING,

    SLIDING,

    JUMPING,
    BACKFLIPPING,
    LONGJUMPING,
    FALLING,
    INITIALLY_FALLING_OFF_LEDGE,

    WALL_SLIDING,
    WALLJUMPING,

    ROLLING,

    STABBING,
    STAB_DASHING,
    STAB_UPSWINGING,
    OVERHEAD_SWINGING,
    UPSWINGING,
  }

  // TODO: Implement behaviors as separate states.
  // TODO: Implement this via a base state machine class.
  public class PlayerStateMachine : IStateMachine<PlayerState> {
    private readonly IStateMachine<PlayerState> impl_ =
        new StateMachine<PlayerState>();

    // TODO: Move these somewhere else.
    public bool WallSlidingOnLeft { get; set; }

    // TODO: Validate that only expected transitions are made.
    public PlayerState State {
      get => this.impl_.State;
      set => this.impl_.State = value;
    }

    public IStateMachine<PlayerState> OnEnter(PlayerState state, Action handler)
      => this.impl_.OnEnter(state, handler);

    public bool IsOnGround => this.IsInState_(PlayerState.STANDING,
                                              PlayerState.WALKING,
                                              PlayerState.RUNNING,
                                              PlayerState.TURNING,
                                              PlayerState.STOPPING,
                                              PlayerState.DUCKING,
                                              PlayerState.DUCKWALKING,
                                              PlayerState.LANDING,
                                              PlayerState.SLIDING);

    public bool CanJumpFromGround => this.IsInState_(
        PlayerState.STANDING,
        PlayerState.WALKING,
        PlayerState.RUNNING,
        PlayerState.TURNING,
        PlayerState.STOPPING,
        PlayerState.DUCKING,
        PlayerState.DUCKWALKING,
        PlayerState.LANDING,
        PlayerState.SLIDING,
        PlayerState.INITIALLY_FALLING_OFF_LEDGE);

    public bool IsMovingUprightOnGround => this.IsInState_(PlayerState.WALKING,
                                                           PlayerState.RUNNING,
                                                           PlayerState.TURNING,
                                                           PlayerState
                                                               .STOPPING);

    public bool IsMovingDuckedOnGround => this.IsInState_(
        PlayerState.DUCKWALKING,
        PlayerState.SLIDING);

    public bool CanMoveUprightOnGround => this.IsInState_(
        PlayerState.STANDING,
        PlayerState.WALKING,
        PlayerState.RUNNING,
        PlayerState.TURNING,
        PlayerState.STOPPING,
        PlayerState.LANDING);

    public bool CanDuck => this.State == PlayerState.RUNNING ||
                           this.IsMovingUprightOnGround ||
                           this.CanMoveUprightOnGround;

    public bool CanMoveDuckedOnGround => this.IsInState_(PlayerState.DUCKING,
                                                         PlayerState
                                                             .DUCKWALKING);

    public bool IsDucked => this.IsInState_(PlayerState.DUCKING,
                                            PlayerState.DUCKWALKING,
                                            PlayerState.SLIDING);

    public bool IsInAir => this.IsInState_(
        PlayerState.JUMPING,
        PlayerState.BACKFLIPPING,
        PlayerState.LONGJUMPING,
        PlayerState.FALLING,
        PlayerState.INITIALLY_FALLING_OFF_LEDGE,
        PlayerState.WALL_SLIDING,
        PlayerState.WALLJUMPING);

    public bool CanMoveInAir => this.IsInState_(
        PlayerState.JUMPING,
        PlayerState.BACKFLIPPING,
        PlayerState.LONGJUMPING,
        PlayerState.FALLING,
        PlayerState.INITIALLY_FALLING_OFF_LEDGE,
        PlayerState.WALL_SLIDING,
        PlayerState.WALLJUMPING);

    public bool IsJumping => this.IsInState_(PlayerState.JUMPING,
                                             PlayerState.BACKFLIPPING,
                                             PlayerState.LONGJUMPING,
                                             PlayerState.WALLJUMPING);

    public bool CanStallJumpingMomentum =>
        this.IsInState_(PlayerState.JUMPING,
                        PlayerState.BACKFLIPPING,
                        PlayerState.LONGJUMPING,
                        PlayerState.WALL_SLIDING,
                        PlayerState.WALLJUMPING);

    public bool IsMovingUpwardInAirAndCanFall =>
        this.IsInState_(PlayerState.JUMPING,
                        PlayerState.BACKFLIPPING,
                        PlayerState.WALLJUMPING);

    private bool IsInState_(params PlayerState[] states) =>
        states.Contains(this.State);
  }
}