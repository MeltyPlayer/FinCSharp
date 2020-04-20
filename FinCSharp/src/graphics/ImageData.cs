using fin.data.collections.grid;
using fin.graphics.color;

namespace fin.graphics {
  public enum ImageType {
    GRAYSCALE,
    RGB,
    RGBA,
  }

  public class ImageData {
    public ImageType imageType { get; }
    public IFinGrid<Color> pixels { get; }

    public ImageData(ImageType imageType, IFinGrid<Color> pixels) {
      this.imageType = imageType;
      this.pixels = pixels;
    }
  }
}