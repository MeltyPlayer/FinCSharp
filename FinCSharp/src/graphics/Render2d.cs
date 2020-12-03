using fin.math.geometry;

namespace fin.graphics {
  using System;

  // TODO: Fix this.
  [Obsolete("Render2d is slow, VBOs should be used instead.", false)]
  public class Render2d {
    private readonly IGraphics graphics_;

    public Render2d(IGraphics graphics) {
      this.graphics_ = graphics;
    }

    public virtual Render2d Point(IVector2<float> vertex) => this.Points(vertex);

    public virtual Render2d Points(params IVector2<float>[] vertices) {
      this.graphics_.Primitives.Begin(PrimitiveType.POINTS)
          .Vertices(vertices)
          .End();
      return this;
    }

    public virtual Render2d Line(params IVector2<float>[] vertices) {
      this.graphics_.Primitives.Begin(PrimitiveType.LINE_STRIP)
          .Vertices(vertices)
          .End();
      return this;
    }

    public virtual Render2d Line(float x1, float y1, float x2, float y2) {
      this.graphics_.Primitives.Begin(PrimitiveType.LINE_STRIP)
          .Vertex(x1, y1)
          .Vertex(x2, y2)
          .End();
      return this;
    }


    public virtual Render2d Circle(
        float centerX,
        float centerY,
        float radius,
        int vertexCount,
        bool filled) {
      var primitives = this.graphics_.Primitives;

      if (filled) {
        primitives.Begin(PrimitiveType.TRIANGLE_FAN);
        primitives.Vertex(centerX, centerY);
        for (var i = 0; i < vertexCount + 1; ++i) {
          var deg = MathF.PI * 2 * i / vertexCount;
          var (x, y) = (centerX + radius * MathF.Cos(deg),
                        centerY - radius * MathF.Sin(deg));
          primitives.Vertex(x, y);
        }
      } else {
        primitives.Begin(PrimitiveType.LINE_LOOP);
        for (var i = 0; i < vertexCount; ++i) {
          var deg = MathF.PI * 2 * i / vertexCount;
          var (x, y) = (centerX + radius * MathF.Cos(deg),
                        centerY - radius * MathF.Sin(deg));
          primitives.Vertex(x, y);
        }
      }

      primitives.End();

      return this;
    }

    public virtual Render2d Rectangle(
        float leftX,
        float topY,
        float width,
        float height,
        bool filled) {
      this.graphics_.Primitives
          .Begin(filled ? PrimitiveType.QUADS : PrimitiveType.LINE_LOOP)
          .VertexUv(0, 0)
          .Vertex(leftX, topY)
          .VertexUv(1, 0)
          .Vertex(leftX + width, topY)
          .VertexUv(1, 1)
          .Vertex(leftX + width, topY + height)
          .VertexUv(0, 1)
          .Vertex(leftX, topY + height)
          .End();
      return this;
    }
  }
}