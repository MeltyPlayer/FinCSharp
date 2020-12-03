using System.Drawing;

using fin.graphics.color;
using fin.math.geometry;

namespace fin.graphics {
  public enum PrimitiveType {
    POINTS,

    LINES,
    LINE_STRIP,
    LINE_LOOP,

    TRIANGLES,
    TRIANGLE_STRIP,
    TRIANGLE_FAN,

    QUADS,
  }

  public abstract class IPrimitives {
    public abstract IPrimitives Begin(PrimitiveType primitiveType);

    public abstract IPrimitives End();

    public virtual IPrimitives Vertex(IVector2<float> v)
      => this.Vertex(v.X, v.Y);

    public abstract IPrimitives Vertex(float x, float y);

    public virtual IPrimitives Vertices(params IVector2<float>[] vs) {
      foreach (var v in vs) {
        this.Vertex(v);
      }

      return this;
    }

    public virtual IPrimitives VertexColor(IColor color) =>
        this.VertexColorB(color.Rb, color.Gb, color.Bb, color.Ab);

    public virtual IPrimitives VertexColor(Color cColor) =>
        this.VertexColorB(cColor.R, cColor.G, cColor.B, cColor.A);

    public abstract IPrimitives VertexColorB(byte r, byte g, byte b, byte a);

    // TODO: Doubles or float?
    public virtual IPrimitives VertexUv((double, double) uv) =>
        this.VertexUv(uv.Item1, uv.Item2);

    public abstract IPrimitives VertexUv(double u, double v);
  }
}