using System.Collections.Generic;

using fin.graphics.color;

using OpenTK.Graphics.OpenGL;

using OpenTkPrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace fin.graphics.impl.opentk {
  public class OpenTkPrimitives : IPrimitives {
    private readonly IDictionary<PrimitiveType, OpenTkPrimitiveType>
      finPrimitiveTypeToOpenTkPrimitiveType_ =
        new Dictionary<PrimitiveType, OpenTkPrimitiveType>() {
          {PrimitiveType.POINTS, OpenTkPrimitiveType.Points},

          {PrimitiveType.LINES, OpenTkPrimitiveType.Lines},
          {PrimitiveType.LINE_STRIP, OpenTkPrimitiveType.LineStrip},
          {PrimitiveType.LINE_LOOP, OpenTkPrimitiveType.LineLoop},

          {PrimitiveType.TRIANGLES, OpenTkPrimitiveType.Triangles},
          {PrimitiveType.TRIANGLE_STRIP, OpenTkPrimitiveType.TriangleStrip},
          {PrimitiveType.TRIANGLE_FAN, OpenTkPrimitiveType.TriangleFan},

          {PrimitiveType.QUADS, OpenTkPrimitiveType.Quads},
        };

    public override IPrimitives Begin(PrimitiveType primitiveType) {
      GL.Begin(this.finPrimitiveTypeToOpenTkPrimitiveType_[primitiveType]);
      return this;
    }

    public override IPrimitives End() {
      GL.End();
      return this;
    }

    public override IPrimitives Vertex(float x, float y) {
      GL.Vertex2(x, y);
      return this;
    }

    public override IPrimitives
      VertexColorB(byte r, byte g, byte b, byte a) {
      GL.Color4(r, g, b, a);
      return this;
    }

    public override IPrimitives VertexUv(double u, double v) {
      GL.TexCoord2(u, v);
      return this;
    }
  }
}