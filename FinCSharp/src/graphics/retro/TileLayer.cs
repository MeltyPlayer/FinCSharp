using fin.data.collections.grid;

namespace fin.graphics.retro {
  class TileLayer<TTileKey> {
    private readonly TileMap<TTileKey> tileMap_;
    private readonly IGrid<TTileKey> tileKeyGrid_;

    public TileLayer(TileMap<TTileKey> tileMap, int width, int height) {
      tileMap_ = tileMap;
      tileKeyGrid_ = new ArrayGrid<TTileKey>(width, height);
    }

    public void Render(Canvas canvas) {
      for (int y = 0; y < tileKeyGrid_.Height; ++y) {
        for (int x = 0; x < tileKeyGrid_.Width; ++x) {
          TTileKey tileKey = tileKeyGrid_[x, y];
          tileMap_.render(canvas, tileKey, x, y);
        }
      }
    }
  }
}