using fin.graphics.color;
using fin.math.geometry;

namespace fin.graphics {
  public interface IScreen {
    IScreen Clear();
    IScreen Clear(IColor color);
  }
}