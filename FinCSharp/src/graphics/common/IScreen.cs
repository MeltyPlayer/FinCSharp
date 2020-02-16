using fin.graphics.common.color;

namespace fin.graphics.common {

  public interface IScreen {
    public IScreen Clear();
    public IScreen Clear(IColor color);
  }
}