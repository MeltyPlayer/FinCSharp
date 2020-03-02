﻿namespace fin.input.impl.opentk {
  using System;
  using System.Collections.Generic;

  using function;

  using OpenTK.Input;

  using button;

  using ButtonState = button.ButtonState;

  public static class KeyToKeyIdConverterOpenTk {
    public static KeyId Convert(Key key) =>
        key switch {
            Key.A => KeyId.A,
            Key.B => KeyId.B,
            Key.C => KeyId.C,
            Key.D => KeyId.D,
            Key.E => KeyId.E,
            Key.F => KeyId.F,
            Key.G => KeyId.G,
            Key.H => KeyId.H,
            Key.I => KeyId.I,
            Key.J => KeyId.J,
            Key.K => KeyId.K,
            Key.L => KeyId.L,
            Key.M => KeyId.M,
            Key.N => KeyId.N,
            Key.O => KeyId.O,

            Key.Space  => KeyId.SPACEBAR,
            Key.Enter  => KeyId.ENTER,
            Key.Escape => KeyId.ESC,

            // TODO: Add rest.
            _ => KeyId.UNKNOWN,
        };
  }

  public class KeyboardOpenTk : IKeyboard {
    private readonly IDictionary<KeyId, IButtonImplementation> keys_ =
        new Dictionary<KeyId, IButtonImplementation>();

    private readonly IButtonManager buttonManager_;

    public KeyboardOpenTk(IButtonManager buttonManager) {
      this.buttonManager_ = buttonManager;
    }

    IButton IKeyboard.this[KeyId keyId] => this[keyId];
    public IButtonImplementation this[KeyId keyId] {
      get {
        if (!this.keys_.TryGetValue(keyId, out var key)) {
          this.keys_[keyId] = key = this.buttonManager_.New();
        }
        return key;
      }
    }
  }
}