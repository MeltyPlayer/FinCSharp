using System;
using System.Collections.Generic;

using fin.data.collections.dictionary;

namespace simple.platformer.player {
  public interface IStateMachine<TState> where TState : notnull {
    TState State { get; set; }

    IStateMachine<TState> OnEnter(TState state, Action handler);
  }

  public class StateMachine<TState> : IStateMachine<TState> where TState : notnull {
    // TODO: This will require a lot of O(log n) lookups. If this becomes a
    // bottleneck, we can speed this up by storing the "onEnter" handler in an
    // IState interface.
    // TODO: We should also be able to change this to O(1) if we convert enums
    // to an int and access these in a list/array.
    private readonly IMultiDictionary<TState, Action> onEnterHandlers_ =
        new MultiDictionary<TState, Action>();

    public TState State {
      get => this.state_;
      set {
        var newState = value;
        if (!this.state_.Equals(newState)) {
          var handlers = this.onEnterHandlers_.TryGet(newState);
          if (handlers != null) {
            foreach (var handler in handlers) {
              handler.Invoke();
            }
          }
        }
        this.state_ = newState;
      }
    }

    private TState state_;

    public IStateMachine<TState> OnEnter(TState state, Action handler) {
      this.onEnterHandlers_.Add(state, handler);
      return this;
    }
  }
}