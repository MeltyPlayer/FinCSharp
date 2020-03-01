using fin.graphics.color;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace fin.graphics.impl.opentk {
  public class ScreenOpenTk : IScreen {
    public IScreen Clear() {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      return this;
    }

    public IScreen Clear(IColor color) {
      GL.ClearColor(new Color4(color.Rf, color.Gf, color.Bf, color.Af));
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      return this;
    }
  }
}