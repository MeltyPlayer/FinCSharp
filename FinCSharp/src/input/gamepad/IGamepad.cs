using System.Collections.Immutable;

using fin.input.analogstick;
using fin.input.button;

namespace fin.input.gamepad {
  public enum AnalogStickType {
    PRIMARY,
    SECONDARY,
  }

  public enum FaceButtonType {
    PRIMARY,
    SECONDARY,
    START,
    SELECT,
  }

  public interface IGamepad {
    bool IsConnected { get; }

    ImmutableArray<IAnalogStick> AnalogSticks { get; }
    IAnalogStick this[AnalogStickType analogStickType] { get; }

    ImmutableArray<IButton> Buttons { get; }
    IButton this[FaceButtonType faceButtonType] { get; }

    // TODO: Remove this.
    void Poll();
  }
}