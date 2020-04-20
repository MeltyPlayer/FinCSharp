using fin.data.collections.grid;
using fin.graphics.color;

namespace fin.graphics {
  // Should be discardable.
  public interface ITexture {
    // TODO: Rethink these, not necessarily possible for all libraries.
    IColor GetPixel(int x, int y);
    IFinGrid<IColor> GetAllPixels();

    // TODO: Rethink these, bad design!!
    void Bind();
    void Unbind();
  }
}