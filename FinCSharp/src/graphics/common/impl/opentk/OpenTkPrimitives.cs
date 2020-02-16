using System.Collections.Generic;

using fin.graphics.common.color;

using OpenTK.Graphics.OpenGL;

using OpenTkPrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace fin.graphics.common.impl.opentk {

  public class OpenTkPrimitives : IPrimitives {

    private readonly IDictionary<PrimitiveType, OpenTkPrimitiveType> finPrimitiveTypeToOpenTkPrimitiveType_ = new Dictionary<PrimitiveType, OpenTkPrimitiveType>()
        {
            { PrimitiveType.POINTS, OpenTkPrimitiveType.Points },

            { PrimitiveType.LINES, OpenTkPrimitiveType.Lines },
            { PrimitiveType.LINE_STRIP, OpenTkPrimitiveType.LineStrip },
            { PrimitiveType.LINE_LOOP, OpenTkPrimitiveType.LineLoop },

            { PrimitiveType.TRIANGLES, OpenTkPrimitiveType.Triangles },
            { PrimitiveType.TRIANGLE_STRIP, OpenTkPrimitiveType.TriangleStrip },
            { PrimitiveType.TRIANGLE_FAN, OpenTkPrimitiveType.TriangleFan },

            { PrimitiveType.QUADS, OpenTkPrimitiveType.Quads },
        };

    public override IPrimitives Begin(PrimitiveType primitiveType) {
      GL.Begin(this.finPrimitiveTypeToOpenTkPrimitiveType_[primitiveType]);
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