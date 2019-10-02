using System.Collections.Generic;

namespace fin.input {

  public enum KeyId {
    UNKNOWN,

    A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
    NO_1, NO_2, NO_3, NO_4, NO_5, NO_6, NO_7, NO_8, NO_9,

    SPACEBAR, ENTER, SHIFT, CTRL, ALT
  }

  public interface IKeyStateDictionary {
    ButtonState this[KeyId keyId] { get; }

    void OnKeyDown(KeyId keyId);

    void OnKeyUp(KeyId keyId);

    void HandleTransitions();
  }

  public class KeyStateDictionary : IKeyStateDictionary {
    private readonly IDictionary<KeyId, ButtonState> keyIdToState_ = new Dictionary<KeyId, ButtonState>();

    private readonly ISet<KeyId> instantaneousKeyIds_ = new HashSet<KeyId>();
    private readonly IDictionary<KeyId, ButtonState> keyIdToTransitionalState_ = new Dictionary<KeyId, ButtonState>();

    public ButtonState this[KeyId keyId] => this.keyIdToState_.TryGetValue(keyId, out ButtonState state) ? state : ButtonState.UP;

    public void OnKeyDown(KeyId keyId) => this.keyIdToTransitionalState_.Add(keyId, ButtonState.DOWN);

    public void OnKeyUp(KeyId keyId) => this.keyIdToTransitionalState_.Add(keyId, ButtonState.UP);

    public void HandleTransitions() {
      // Handled instantaneous states.
      foreach (var instanteousKeyId in this.instantaneousKeyIds_) {
        var instantaneousState = this[instanteousKeyId];
        var newState = instantaneousState == ButtonState.PRESSED ? ButtonState.DOWN : ButtonState.UP;

        this.keyIdToState_.Add(instanteousKeyId, newState);
      }
      this.instantaneousKeyIds_.Clear();

      // Handle transition states.
      foreach (var keyIdAndTransitionalState in this.keyIdToTransitionalState_) {
        var keyId = keyIdAndTransitionalState.Key;
        var transition = keyIdAndTransitionalState.Value;

        var oldState = this[keyId];

        var newState = (transition == ButtonState.UP) ?
                         (oldState == ButtonState.PRESSED || oldState == ButtonState.DOWN) ? ButtonState.RELEASED : ButtonState.DOWN
                       :
                         (oldState == ButtonState.RELEASED || oldState == ButtonState.UP) ? ButtonState.PRESSED : ButtonState.UP;

        if (newState == ButtonState.RELEASED || newState == ButtonState.PRESSED) {
          this.instantaneousKeyIds_.Add(keyId);
        }

        if (newState != ButtonState.UP) {
          this.keyIdToState_.Add(keyId, newState);
        } else {
          this.keyIdToState_.Remove(keyId);
        }
      }
      this.keyIdToTransitionalState_.Clear();
    }
  }
}