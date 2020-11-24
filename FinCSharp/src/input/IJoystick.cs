using System.Collections.Immutable;

using fin.input.button;

namespace fin.input {
  public interface IJoystick {
    bool IsConnected { get; }
    ImmutableArray<IAxis> Axes { get; }
    ImmutableArray<IButton> Buttons { get; }
  }
}