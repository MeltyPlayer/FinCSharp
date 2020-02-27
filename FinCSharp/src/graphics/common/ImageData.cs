using fin.data.collections.grid;
using fin.graphics.common.color;

namespace fin.graphics.common {
  public enum ImageType {
    GRAYSCALE,
    RGB,
    RGBA,
  }

  public struct ImageData {
    public ImageType imageType { get; }
    public IGrid<Color> pixels { get; }

    public ImageData(ImageType imageType, IGrid<Color> pixels) {
      this.imageType = imageType;
      this.pixels = pixels;
    }
  }
}