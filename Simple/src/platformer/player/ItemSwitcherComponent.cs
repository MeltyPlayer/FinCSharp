using fin.app;
using fin.app.events;
using fin.app.node;
using fin.graphics.camera;
using fin.input;
using fin.math.number;

namespace simple.platformer.player {
  public interface IItemComponent : IComponent {
    [OnTick]
    void ProcessInputs(ProcessInputsEvent _);

    [OnTick]
    void TickCollisions(TickCollisionsEvent _);

    [OnTick]
    void RenderForOrthographicCamera(
        RenderForOrthographicCameraTickEvent evt);
  }

  public class ItemSwitcherComponent : IItemComponent {
    private IGamepad gamepad_;

    private IItemComponent[] itemComponents_;
    private IRangedInt currentItemIndex_;

    public ItemSwitcherComponent(
        IGamepad gamepad,
        PlayerRigidbody playerRigidbody,
        PlayerStateMachine stateMachine) {
      this.gamepad_ = gamepad;

      this.itemComponents_ = new IItemComponent[] {
          new ShieldComponent(gamepad,
                              playerRigidbody,
                              stateMachine),
          new WhipComponent(gamepad,
                            playerRigidbody,
                            stateMachine),
          new SwordComponent(gamepad,
                             playerRigidbody,
                             stateMachine),
      };
      this.currentItemIndex_ =
          new CircularRangedInt(0, 0, this.itemComponents_.Length);
    }

    private IItemComponent CurrentItem
      => this.itemComponents_[this.currentItemIndex_.Value];

    [OnTick]
    public void ProcessInputs(ProcessInputsEvent _) {
      if (this.gamepad_[FaceButtonType.SECONDARY].IsPressed) {
        ++this.currentItemIndex_.Value;
      }

      this.CurrentItem.ProcessInputs(_);
    }

    [OnTick]
    public void TickCollisions(TickCollisionsEvent _)
      => this.CurrentItem.TickCollisions(_);

    [OnTick]
    public void RenderForOrthographicCamera(
        RenderForOrthographicCameraTickEvent evt)
      => this.CurrentItem.RenderForOrthographicCamera(evt);
  }
}