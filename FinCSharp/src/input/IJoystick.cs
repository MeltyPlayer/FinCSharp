using System.Collections.Immutable;

using fin.input.button;

namespace fin.input {
  public interface IJoystick {
    bool IsConnected { get; }
    ImmutableArray<IDpad> Dpads { get; }
    ImmutableArray<IAnalogStick> AnalogSticks { get; }
    ImmutableArray<IButton> Buttons { get; }
  }
}