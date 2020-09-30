using fin.events;
using fin.graphics;
using fin.input;

namespace fin.app {
  /// <summary>
  ///   Event triggered at the start of each tick.
  /// </summary>
  public class StartTickEvent : BEvent {}

  /// <summary>
  ///   Event for processing user input from the keyboard/mouse/gamepad.
  /// </summary>
  public class ProcessInputsEvent : BEvent {
    public IInput Input { get; }

    public ProcessInputsEvent(IInput input) {
      this.Input = input;
    }
  }

  public class TickPhysicsEvent : BEvent {}

  public class TickCollisionsEvent : BEvent {}

  public class TickAnimationEvent : BEvent {}

  /// <summary>
  /// Event to be used by cameras, where they will trigger a Render event to be
  /// handled by children (e.g.RenderForOrthographicCameraTickEvent).
  /// </summary>
  public class TriggerRenderViewsTickEvent : BEvent {
    public IGraphics Graphics { get; }

    public TriggerRenderViewsTickEvent(IGraphics graphics) {
      this.Graphics = graphics;
    }
  }
}