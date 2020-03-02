namespace fin.input.impl.opentk {
  using System.Collections.Generic;

  using button;

  /// <summary>
  ///   The internal controller of a button. Can transition to "down" or "up",
  ///   will automatically track "pressed" and "released".
  /// </summary>
  public interface IButtonImplementation : IButton {
    void Down();
    void Up();
  }

  public interface IButtonManager {
    /// <summary>
    ///   Creates a new button implementation that will automatically
    ///   transition. This should be wrapped!
    /// </summary>
    IButtonImplementation New();
  }

  public class ButtonManagerOpenTk : IButtonManager {
    private readonly ISet<ButtonImplementationOpenTk> impls_ =
        new HashSet<ButtonImplementationOpenTk>();

    private readonly ISet<(ButtonImplementationOpenTk, ButtonState)>
        transitions_ =
            new HashSet<(ButtonImplementationOpenTk, ButtonState)>();

    private readonly ISet<ButtonImplementationOpenTk>
        instantaneousStates_ =
            new HashSet<ButtonImplementationOpenTk>();

    public void HandleTransitions() {
      foreach (var button in this.instantaneousStates_) {
        button.State = button.State == ButtonState.PRESSED
                           ? ButtonState.DOWN
                           : ButtonState.UP;
      }
      this.instantaneousStates_.Clear();

      foreach (var (button, newState) in this.transitions_) {
        button.State = newState;
        this.instantaneousStates_.Add(button);
      }
      this.transitions_.Clear();
    }

    public IButtonImplementation New() {
      var buttonImplementation = new ButtonImplementationOpenTk(this);
      this.impls_.Add(buttonImplementation);
      return buttonImplementation;
    }

    private class ButtonImplementationOpenTk : IButtonImplementation {
      private readonly ButtonManagerOpenTk parent_;
      public ButtonState State { get; set; } = ButtonState.RELEASED;

      public ButtonImplementationOpenTk(ButtonManagerOpenTk parent) {
        this.parent_ = parent;
      }

      public void Down() {
        var asBase = (IButtonImplementation) this;
        if (asBase.IsUp) {
          this.parent_.transitions_.Add((this, ButtonState.PRESSED));
        }
      }

      public void Up() {
        var asBase = (IButtonImplementation) this;
        if (asBase.IsDown) {
          this.parent_.transitions_.Add((this, ButtonState.RELEASED));
        }
      }
    }
  }
}