using fin.graphics.common.color;
using fin.log;
using fin.math.geometry;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace fin.graphics.common.impl.opentk {

  public class ScreenOpentk : IScreen {
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