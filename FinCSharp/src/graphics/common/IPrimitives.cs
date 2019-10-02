namespace fin.graphics.common {

  public enum PrimitiveType {
    POINTS,

    LINE_STRIP,
    LINE_LOOP,

    TRIANGLES,

    QUADS,
  }

  public abstract class IPrimitives {

    public abstract IPrimitives Begin(PrimitiveType primitiveType);

    public abstract IPrimitives End();

    public abstract IPrimitives V(Vertex2d v);

    public virtual IPrimitives Vs(params Vertex2d[] vs) {
      foreach (var v in vs) {
        this.V(v);
      }
      return this;
    }

    public abstract IPrimitives C(Color color);

    public abstract IPrimitives U(double r, double g, double b, double a);
  }
}