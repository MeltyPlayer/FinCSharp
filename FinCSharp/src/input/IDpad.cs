using System;
using System.Collections.Generic;
using System.Text;

namespace fin.input {

  public interface IDpad {
    ButtonState UpState { get; }
    ButtonState DownState { get; }
    ButtonState LeftState { get; }
    ButtonState RightState { get; }
  }
}