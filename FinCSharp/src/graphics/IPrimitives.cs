using fin.graphics.color;

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

    public virtual IPrimitives Vertex(Vertex2d v) => this.Vertex(v.x, v.y);
    public abstract IPrimitives Vertex(float x, float y);

    public virtual IPrimitives Vertices(params Vertex2d[] vs) {
      foreach (var v in vs) {
        this.Vertex(v);
      }

      return this;
    }

    public virtual IPrimitives VertexColor(Color color) =>
      this.VertexColorB(color.Rb, color.Gb, color.Bb, color.Ab);

    public abstract IPrimitives VertexColorB(byte r, byte g, byte b, byte a);
  }
}