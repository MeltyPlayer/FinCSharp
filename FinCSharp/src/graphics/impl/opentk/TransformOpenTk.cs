using OpenTK.Graphics.OpenGL;

namespace fin.graphics.impl.opentk {
  // TODO: Allow using transforms on other than modelview matrix?
  public class TransformOpenTk : ITransform {
    public ITransform Identity() {
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
      return this;
    }

    public ITransform Push() {
      GL.MatrixMode(MatrixMode.Modelview);
      GL.PushMatrix();
      return this;
    }

    public ITransform Pop() {
      GL.MatrixMode(MatrixMode.Modelview);
      GL.PopMatrix();
      return this;
    }


    // 2D transforms
    public ITransform Translate(float x, float y) {
      GL.MatrixMode(MatrixMode.Modelview);
      GL.Translate(x, y, 0);
      return this;
    }

    public ITransform Rotate(float deg) {
      GL.MatrixMode(MatrixMode.Modelview);
      GL.Rotate(-deg, 0, 0, 1);
      return this;
    }
  }
}