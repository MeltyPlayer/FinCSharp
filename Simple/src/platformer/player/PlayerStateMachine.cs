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
    SLIDING,

    JUMPING,
    BACKFLIPPING,
    LONGJUMPING,
    FALLING,

    SWINGING,
  }

  public class PlayerStateMachine {
    // TODO: Validate that only expected transitions are made.
    public PlayerState State { get; set; }

    public bool IsOnGround => this.IsInState_(PlayerState.STANDING,
                                              PlayerState.WALKING,
                                              PlayerState.RUNNING,
                                              PlayerState.TURNING,
                                              PlayerState.STOPPING,
                                              PlayerState.DUCKING,
                                              PlayerState.DUCKWALKING,
                                              PlayerState.SLIDING);

    public bool IsMovingUprightOnGround => this.IsInState_(PlayerState.WALKING,
                                                           PlayerState.RUNNING,
                                                           PlayerState.TURNING,
                                                           PlayerState
                                                               .STOPPING);

    public bool IsMovingDuckedOnGround => this.IsInState_(
        PlayerState.DUCKWALKING,
        PlayerState.SLIDING);

    public bool CanMoveUprightOnGround => this.IsInState_(PlayerState.STANDING,
                                                          PlayerState.WALKING,
                                                          PlayerState.RUNNING,
                                                          PlayerState.TURNING,
                                                          PlayerState.STOPPING);

    public bool CanMoveDuckedOnGround => this.IsInState_(PlayerState.DUCKING,
                                                         PlayerState
                                                             .DUCKWALKING);

    public bool IsDucked => this.IsInState_(PlayerState.DUCKING,
                                            PlayerState.DUCKWALKING,
                                            PlayerState.SLIDING);

    public bool IsInAir => this.IsInState_(PlayerState.JUMPING,
                                           PlayerState.BACKFLIPPING,
                                           PlayerState.LONGJUMPING,
                                           PlayerState.FALLING);

    public bool CanMoveInAir => this.IsInState_(PlayerState.JUMPING,
                                                PlayerState.BACKFLIPPING,
                                                PlayerState.LONGJUMPING,
                                                PlayerState.FALLING);

    public bool IsJumping =>
        this.IsInState_(PlayerState.JUMPING,
                        PlayerState.BACKFLIPPING,
                        PlayerState.LONGJUMPING);

    public bool IsMovingUpwardInAirAndCanFall =>
        this.IsInState_(PlayerState.JUMPING,
                        PlayerState.BACKFLIPPING);

    private bool IsInState_(params PlayerState[] states) =>
        states.Contains(this.State);
  }
}