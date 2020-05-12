using System.Drawing;

using fin.data.collections.grid;
using fin.file;

namespace simple.platformer.world {
  public class LevelConstants {
    public const double SIZE = 32;
    public static readonly IFinGrid<bool> LEVEL;

    static LevelConstants() {
      var bitmap =
          (Bitmap) Image.FromFile(LocalFile.WithinResources("level.bmp").uri);

      var (width, height) = (bitmap.Width, bitmap.Height);
      LevelConstants.LEVEL =
          new FinArrayGrid<bool>(bitmap.Width, bitmap.Height, false) {
              ShouldThrowExceptions = false,
          };

      for (var y = 0; y < height; ++y) {
        for (var x = 0; x < width; ++x) {
          var color = bitmap.GetPixel(x, y);
          if (color.R == 0) {
            LevelConstants.LEVEL[x, y] = true;
          }
        }
      }
    }
  }
}