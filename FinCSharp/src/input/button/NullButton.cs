﻿namespace fin.input.button {
  public class NullButton : IButton {
    public static IButton Instance { get; } = new NullButton();

    private NullButton() { }

    public ButtonState State => ButtonState.UP;
  }
}