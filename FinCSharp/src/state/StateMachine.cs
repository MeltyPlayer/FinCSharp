using System.Linq;

namespace fin.state {
  public class StateMachine<TState> where TState : notnull {
    public StateMachine(TState startingState) {
      this.CurrentState = startingState;
    }

    public TState CurrentState { private set; get; }

    public bool Check(params TState[] checkStates) =>
      checkStates.Contains(this.CurrentState);

    public void GoTo(TState newState) {
      var oldState = this.CurrentState;
      this.CurrentState = newState;

      this.OnExitEvent(oldState);
      this.OnEnterEvent(newState);
      this.OnTransitionEvent(oldState, newState);
    }

    public delegate void OnEnterEventHandler(TState newState);

    private event OnEnterEventHandler OnEnterEvent = delegate { };

    public delegate void OnExitEventHandler(TState oldState);

    private event OnExitEventHandler OnExitEvent = delegate { };

    public delegate void OnTransitionEventHandler(TState oldState,
      TState newState);

    private event OnTransitionEventHandler OnTransitionEvent = delegate { };
  }
}