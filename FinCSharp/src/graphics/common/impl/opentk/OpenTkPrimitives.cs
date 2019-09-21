using OpenTK.Graphics.OpenGL;

namespace fin.graphics.common.impl.opentk {
  public class OpenTkPrimitives : IPrimitives {
    public override IPrimitives Begin(PrimitiveType primitiveType) {
      switch (primitiveType) {
        case PrimitiveType.POINTS:
          GL.Begin(BeginMode.Points);
          break;
        case PrimitiveType.LINE_STRIP:
          GL.Begin(BeginMode.LineStrip);
          break;
        case PrimitiveType.TRIANGLES:
          GL.Begin(BeginMode.Triangles);
          break;
        case PrimitiveType.QUADS:
          GL.Begin(BeginMode.Quads);
          break;
      }
      return this;
    }

    public override IPrimitives End() {
      GL.End();
      return this;
    }

    public override IPrimitives V(Vertex2d v) {
      GL.Vertex2(v.x, v.y);
      return this;
    }

    public override IPrimitives Vs(params Vertex2d[] vs) {
      foreach (var v in vs) {
        this.V(v);
      }
      return this;
    }

    public override IPrimitives C(Color color) {
      return this;
    }

    public override IPrimitives U(double r, double g, double b, double a) {
      return this;
    }
  }
}