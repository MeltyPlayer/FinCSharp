using fin.data.collections.grid;
using fin.graphics.color;

namespace fin.graphics {
  public interface ITexture {
    IColor GetPixel(int x, int y);

    IGrid<IColor> GetAllPixels();
  }
}