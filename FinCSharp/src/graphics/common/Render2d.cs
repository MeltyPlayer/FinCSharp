namespace fin.graphics.common {
  public class Render2d {
    private readonly IGraphics g_;

    public Render2d(IGraphics g) {
      this.g_ = g;
    }

    public virtual Render2d Point(Vertex2d vertex) => this.Points(vertex);

    public virtual Render2d Points(params Vertex2d[] vertices) {
      this.g_.Primitives.Begin(PrimitiveType.POINTS).Vs(vertices).End();
      return this;
    }

    public virtual Render2d Line(params Vertex2d[] vertices) {
      this.g_.Primitives.Begin(PrimitiveType.LINE_STRIP).Vs(vertices).End();
      return this;
    }
  }
}