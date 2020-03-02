using System.Collections.Generic;

namespace fin.input {
  using button;

  public enum KeyId {
    UNKNOWN,

    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,
    NO_1,
    NO_2,
    NO_3,
    NO_4,
    NO_5,
    NO_6,
    NO_7,
    NO_8,
    NO_9,

    SPACEBAR,
    ENTER,
    SHIFT,
    CTRL,
    ALT,
    ESC
  }

  public interface IKeyboard {
    IButton this[KeyId keyId] { get; }
  }
}