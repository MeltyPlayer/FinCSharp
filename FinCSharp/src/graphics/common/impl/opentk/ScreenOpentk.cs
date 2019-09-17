using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace fin.graphics.common.impl.opentk {
  public class ScreenOpentk : IScreen {
    public override IScreen Clear() {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      return this;
    }

    public override IScreen Clear(Color color) {
      GL.ClearColor(new Color4(color.Rf, color.Gf, color.Bf, color.Af));
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      return this;
    }
  }
}
