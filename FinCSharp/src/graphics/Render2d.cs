using System;

namespace fin.graphics {
  using OpenTK.Graphics.OpenGL;

  public class Render2d {
    private readonly IGraphics graphics_;

    public Render2d(IGraphics graphics) {
      this.graphics_ = graphics;
    }

    public virtual Render2d Point(Vertex2d vertex) => this.Points(vertex);

    public virtual Render2d Points(params Vertex2d[] vertices) {
      this.graphics_.Primitives.Begin(PrimitiveType.POINTS).Vertices(vertices)
          .End();
      return this;
    }

    public virtual Render2d Line(params Vertex2d[] vertices) {
      this.graphics_.Primitives.Begin(PrimitiveType.LINE_STRIP)
          .Vertices(vertices)
          .End();
      return this;
    }

    public virtual Render2d Circle(float centerX,
                                   float centerY,
                                   float radius,
                                   int vertexCount,
                                   bool filled) {
      var primitives = this.graphics_.Primitives;

      if (filled) {
        primitives.Begin(PrimitiveType.TRIANGLE_FAN);
        primitives.Vertex(new Vertex2d());
        for (var i = 0; i < vertexCount + 1; ++i) {
          var deg = MathF.PI * 2 * i / vertexCount;
          var (x, y) = (centerX + radius * MathF.Cos(deg),
                        centerY - radius * MathF.Sin(deg));
          primitives.Vertex(x, y);
        }
      }
      else {
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

    public virtual Render2d Rectangle(float leftX,
                                      float topY,
                                      float width,
                                      float height,
                                      bool filled) {
      this.graphics_.Primitives
          .Begin(filled ? PrimitiveType.QUADS : PrimitiveType.LINE_LOOP)
          .Vertex(leftX, topY)
          .Vertex(leftX + width, topY)
          .Vertex(leftX + width, topY + height)
          .Vertex(leftX, topY + height)
          .End();
      return this;
    }
  }
}