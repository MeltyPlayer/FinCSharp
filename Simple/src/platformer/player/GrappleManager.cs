using fin.math.number;

namespace simple.platformer.player {
  public class GrappleManager {
    private enum GrappleState {
      HIDDEN,
      SHOOTING,
      IN_PLACE
    }

    private GrappleState state_ = GrappleState.HIDDEN;
    private IFraction grappleLength_;
    private double grappleX_;
    private double grappleY_;
    private IDirection grappleDirection_;

    private IDirection? scheduledGrappleDirection_;

    public PlayerRigidbody PlayerRigidbody { get; set; }

    /*public void ScheduleFire(IDirection grappleDirection) {
      this.scheduledGrappleDirection_ = grappleDirection.Degrees;
    }

    public void ProcessInputs() {
      this.scheduledGrappleDirection_ = 0;
    }

    public void DrawGrapple() {
      var 
    }*/
  }
}