using System;
using System.Collections.Generic;
using System.Drawing;

using fin.data.collections.grid;
using fin.io;

namespace simple.platformer.world {
  [Flags]
  public enum LevelTileTypes {
    EMPTY = 0,
    FLOOR = 1,
    CEILING = 1 << 2,
    LEFT_WALL = 1 << 3,
    RIGHT_WALL = 1 << 4,
  }

  public class LevelGrid {
    private readonly IFinGrid<LevelTileTypes> tiles_;

    public float Size { get; }

    public LevelGrid(float size) {
      this.Size = size;

      var bitmap =
          (Bitmap) Image.FromFile(LocalIo.Resources.GetFile("level.bmp").Uri);

      var (width, height) = (bitmap.Width, bitmap.Height);
      var boolGrid =
          new FinSparseGrid<bool>(bitmap.Width, bitmap.Height, false) {
              ShouldThrowExceptions = false,
          };
      for (var y = 0; y < height; ++y) {
        for (var x = 0; x < width; ++x) {
          var color = bitmap.GetPixel(x, y);
          if (color.R == 0) {
            boolGrid[x, y] = true;
          }
        }
      }

      this.tiles_ = new FinSparseGrid<LevelTileTypes>(
          bitmap.Width,
          bitmap.Height,
          LevelTileTypes.EMPTY) {
          ShouldThrowExceptions = false,
      };
      foreach (var node in boolGrid) {
        if (!node.Value) {
          continue;
        }

        var (c, r) = (node.C, node.R);

        var tile = LevelTileTypes.EMPTY;

        var isFloor = !boolGrid[c, r - 1];
        if (isFloor) {
          tile |= LevelTileTypes.FLOOR;
        }

        var isCeiling = !boolGrid[c, r + 1];
        if (isCeiling) {
          tile |= LevelTileTypes.CEILING;
        }

        var isLeftWall = !boolGrid[c - 1, r];
        if (isLeftWall) {
          tile |= LevelTileTypes.LEFT_WALL;
        }

        var isRightWall = !boolGrid[c + 1, r];
        if (isRightWall) {
          tile |= LevelTileTypes.RIGHT_WALL;
        }

        this.tiles_[c, r] = tile;
      }

      boolGrid.Clear();
    }

    public IEnumerator<IGridNode<LevelTileTypes>> GetEnumerator() =>
        this.tiles_.GetEnumerator();

    public LevelTileTypes GetAtIndex(int c, int r) => this.tiles_[c, r];

    public bool CheckAtIndex(int c, int r, LevelTileTypes tile)
      => LevelGrid.Matches(this.GetAtIndex(c, r), tile);

    public LevelTileTypes GetAtPosition(double x, double y) {
      var (c, r) = this.GetIndexFromPosition_(x, y);
      return this.GetAtIndex(c, r);
    }

    public bool CheckAtPosition(double x, double y, LevelTileTypes tile)
      => LevelGrid.Matches(this.GetAtPosition(x, y), tile);

    public static bool Matches(LevelTileTypes lhs, LevelTileTypes rhs) {
      var matches = lhs & rhs;
      return matches != LevelTileTypes.EMPTY;
    }

    private (int, int) GetIndexFromPosition_(double x, double y)
      => ((int) Math.Floor(x / this.Size), (int) Math.Floor(y / this.Size));
  }
}