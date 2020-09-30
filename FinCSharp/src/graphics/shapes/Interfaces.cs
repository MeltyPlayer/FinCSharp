using System;
using System.Collections.Generic;
using System.Text;

using fin.math.geometry;
using fin.math.number;

namespace fin.graphics.shapes {
  public interface IShape {
    public IVector2<float> Position { get; }
    public IVector2<float> Scale { get; }
    public IDirection Rotation { get; }

    public IShape? Parent { get; }
    public IEnumerable<IShape>? Children { get; }

    public IShape? GetAtPoint(float x, float y);
  }

  public interface IRectangle : IShape {
    public IBoundingBox<float> BoundingBox { get; }
  }

  public interface IText : IRectangle {
    public string Text { get; set; }
  }
}