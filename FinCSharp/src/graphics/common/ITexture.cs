using fin.data.collections.grid;
using fin.dispose;

namespace fin.graphics.common {
  public abstract class ITexture : IFinDisposable {
    public abstract Color GetPixel(int x, int y);
    public abstract IGrid<Color> GetAllPixels();
  }
}
