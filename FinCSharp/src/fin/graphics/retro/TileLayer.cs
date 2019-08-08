namespace fin.graphics.retro {
  class TileLayer<TTileKey> {
    private readonly TileMap<TTileKey> tileMap_;
    private readonly data.grid.IGrid<TTileKey> tileKeyGrid_;

    public TileLayer(TileMap<TTileKey> tileMap, int width, int height) {
      tileMap_ = tileMap;
      tileKeyGrid_ = new data.grid.ArrayGrid<TTileKey>(width, height);
    }

    public void Render(Canvas canvas) {
      for (int y = 0; y < tileKeyGrid_.height; ++y) {
        for (int x = 0; x < tileKeyGrid_.width; ++x) {
          TTileKey tileKey = tileKeyGrid_[x, y];
          tileMap_.render(canvas, tileKey, x, y);
        }
      }
    }
  }
}