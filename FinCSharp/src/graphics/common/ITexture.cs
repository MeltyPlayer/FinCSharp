using fin.data.collections.grid;

namespace fin.graphics.common {

  public interface ITexture {

    Color GetPixel(int x, int y);

    IGrid<Color> GetAllPixels();
  }
}