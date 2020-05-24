using System;
using System.Collections.Generic;
using System.Text;

using fin.graphics;
using fin.graphics.color;
using fin.math.geometry;

namespace simple.platformer.world {
  public class LevelGridRenderer {
    public LevelGrid LevelGrid { get; set; }

    private MutableBoundingBox<double> cachedBounds_ =
        new MutableBoundingBox<double>();

    public void Render(IGraphics g) {
      var size = this.LevelGrid.Size;

      var primitives = g.Primitives;
      primitives.Begin(PrimitiveType.QUADS);
      foreach (var node in this.LevelGrid) {
        var tile = node.Value;

        if (tile == LevelTileType.EMPTY) {
          continue;
        }

        var color = Color.FromRgbF(
            LevelGrid.Matches(tile, LevelTileType.LEFT_WALL) ||
            LevelGrid.Matches(tile, LevelTileType.RIGHT_WALL)
                ? 1
                : 0,
            LevelGrid.Matches(tile, LevelTileType.FLOOR)
                ? 1
                : 0,
            LevelGrid.Matches(
                tile,
                LevelTileType.CEILING)
                ? 1
                : 0);

        var (c, r) = (node.C, node.R);
        var (x, y) = (size * c, size * r);

        var leftX = (int) x;
        var rightX = (int) (x + size);
        var topY = (int) y;
        var bottomY = (int) (y + size);

        primitives.VertexColor(color).Vertex(leftX, topY)
                  .Vertex(rightX, topY)
                  .Vertex(rightX, bottomY)
                  .Vertex(leftX, bottomY);
      }
      primitives.End();
    }
  }
}