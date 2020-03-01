namespace fin.retro {
  class TileLayer<TTileKey> {
    private readonly TileMap<TTileKey> tileMap_;
    private readonly fin.data.collections.grid.IGrid<TTileKey> tileKeyGrid_;

    public TileLayer(TileMap<TTileKey> tileMap, int width, int height) {
      this.tileMap_ = tileMap;
      this.tileKeyGrid_ = new fin.data.collections.grid.ArrayGrid<TTileKey>(width, height);
    }

    public void Render(Canvas canvas) {
      for (int y = 0; y < this.tileKeyGrid_.Height; ++y) {
        for (int x = 0; x < this.tileKeyGrid_.Width; ++x) {
          TTileKey tileKey = this.tileKeyGrid_[x, y];
          this.tileMap_.render(canvas, tileKey, x, y);
        }
      }
    }
  }
}