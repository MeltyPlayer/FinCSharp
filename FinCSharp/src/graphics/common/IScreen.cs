using fin.graphics.common.color;
using fin.math.geometry;

namespace fin.graphics.common {

  public interface IScreen {
    IScreen Clear();
    IScreen Clear(IColor color);
  }
}