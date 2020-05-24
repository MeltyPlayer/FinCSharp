using System;
using System.Collections.Generic;
using System.Drawing;

using fin.data.collections.grid;
using fin.file;
using fin.math.geometry;

namespace simple.platformer.world {
  [Flags]
  public enum LevelTileType {
    EMPTY = 0,
    FLOOR = 1,
    CEILING = 1 << 2,
    LEFT_WALL = 1 << 3,
    RIGHT_WALL = 1 << 4,
  }

  public class LevelGrid {
    private readonly IFinGrid<LevelTileType> tiles_;

    public double Size { get; }

    public LevelGrid(double size) {
      this.Size = size;

      var bitmap =
          (Bitmap) Image.FromFile(LocalFile.WithinResources("level.bmp").uri);

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

      this.tiles_ = new FinSparseGrid<LevelTileType>(
          bitmap.Width,
          bitmap.Height,
          LevelTileType.EMPTY) {
          ShouldThrowExceptions = false,
      };
      foreach (var node in boolGrid) {
        if (!node.Value) {
          continue;
        }

        var (c, r) = (node.C, node.R);

        var tile = LevelTileType.EMPTY;

        var isFloor = !boolGrid[c, r - 1];
        if (isFloor) {
          tile |= LevelTileType.FLOOR;
        }

        var isCeiling = !boolGrid[c, r + 1];
        if (isCeiling) {
          tile |= LevelTileType.CEILING;
        }

        var isLeftWall = !boolGrid[c - 1, r];
        if (isLeftWall) {
          tile |= LevelTileType.LEFT_WALL;
        }

        var isRightWall = !boolGrid[c + 1, r];
        if (isRightWall) {
          tile |= LevelTileType.RIGHT_WALL;
        }

        this.tiles_[c, r] = tile;
      }

      boolGrid.Clear();
    }

    public IEnumerator<IGridNode<LevelTileType>> GetEnumerator() =>
        this.tiles_.GetEnumerator();

    public LevelTileType GetAtIndex(int c, int r)
      => this.tiles_[c, r];

    public bool CheckAtIndex(int c, int r, LevelTileType tile)
      => LevelGrid.Matches(this.GetAtIndex(c, r), LevelTileType.EMPTY);

    public LevelTileType GetAtPosition(double x, double y) {
      var (c, r) = this.GetIndexFromPosition(x, y);
      return this.GetAtIndex(c, r);
    }

    public bool CheckAtPosition(double x, double y, LevelTileType tile)
      => LevelGrid.Matches(this.GetAtPosition(x, y), LevelTileType.EMPTY);

    public static bool Matches(LevelTileType lhs, LevelTileType rhs) {
      var matches = lhs & rhs;
      return matches != LevelTileType.EMPTY;
    }

    public (int, int) GetIndexFromPosition(double x, double y)
      => ((int) Math.Floor(x / this.Size), (int) Math.Floor(y / this.Size));
  }
}