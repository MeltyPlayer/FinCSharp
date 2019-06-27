using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.graphics.retro {
  class TileLayer<TTileKey> {
    private readonly TileMap<TTileKey> tileMap_;
    private readonly data.Grid<TTileKey> tileKeyGrid_;

    public TileLayer(TileMap<TTileKey> tileMap, int width, int height) {
      tileMap_ = tileMap;
      tileKeyGrid_ = new data.Grid<TTileKey>(width, height);
    }

    public void render(Canvas canvas) {
      for (int y = 0; y < tileKeyGrid_.height; ++y) {
        for (int x = 0; x < tileKeyGrid_.width; ++x) {
          TTileKey tileKey = tileKeyGrid_[x, y];
          tileMap_.render(canvas, tileKey, x, y);
        }
      }
    }
  }
}
