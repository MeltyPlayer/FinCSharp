using System.Linq;

namespace simple.platformer.player {
  public enum PlayerState {
    STANDING,
    WALKING,
    RUNNING,
    TURNING,
    STOPPING,
    DUCKING,
    SLIDING,

    JUMPING,
    BACKFLIPPING,
    FALLING,
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
                                              PlayerState.SLIDING);

    public bool CanMoveOnGround => this.IsInState_(PlayerState.STANDING,
                                                   PlayerState.WALKING,
                                                   PlayerState.RUNNING,
                                                   PlayerState.TURNING,
                                                   PlayerState.STOPPING);

    public bool IsInAir => this.IsInState_(PlayerState.JUMPING,
                                           PlayerState.BACKFLIPPING,
                                           PlayerState.FALLING);

    public bool CanMoveInAir => this.IsInState_(PlayerState.JUMPING,
                                                PlayerState.BACKFLIPPING,
                                                PlayerState.FALLING);

    public bool IsJumping =>
        this.IsInState_(PlayerState.JUMPING, PlayerState.BACKFLIPPING);

    public bool IsMovingUpwardInAir =>
        this.IsInState_(PlayerState.JUMPING, PlayerState.BACKFLIPPING);

    private bool IsInState_(params PlayerState[] states) =>
        states.Contains(this.State);
  }
}