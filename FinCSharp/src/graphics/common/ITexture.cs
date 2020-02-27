using fin.data.collections.grid;
using fin.graphics.common.color;

namespace fin.graphics.common {
  public interface ITexture {
    IColor GetPixel(int x, int y);

    IGrid<IColor> GetAllPixels();
  }
}