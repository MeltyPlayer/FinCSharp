using fin.data.collections.grid;
using fin.discardable;
using fin.graphics.color;

using OpenTK.Graphics.OpenGL;

namespace fin.graphics.impl.opentk {
  public class TexturesOpenTk : ITextures {
    public ITexture Create(ImageData imageData) {
      var textureFormat =
        TexturesOpenTk.ConvertImageTypeToTextureFormat_(imageData.imageType);

      var (width, height) =
        (imageData.pixels.Width, imageData.pixels.Height);

      uint textureId;
      GL.CreateTextures(TextureTarget.Texture2D, 1, out textureId);
      GL.TextureStorage2D(textureId,
                          1,
                          textureFormat,
                          width,
                          height);

      var pixelData =
        TexturesOpenTk.ConvertRgbaGridToUintArray_(imageData.pixels);

      GL.BindTexture(TextureTarget.Texture2D, textureId);
      GL.TextureSubImage2D(textureId,
                           0,
                           0,
                           0,
                           width,
                           height,
                           PixelFormat.Rgba,
                           PixelType.Int,
                           pixelData);
      GL.BindTexture(TextureTarget.Texture2D, 0);

      return new TextureOpentk(textureId);
    }

    private static SizedInternalFormat ConvertImageTypeToTextureFormat_(
      ImageType imageType) {
      switch (imageType) {
        case ImageType.GRAYSCALE:
          return SizedInternalFormat.R8i;

        case ImageType.RGB:
        case ImageType.RGBA:
          return SizedInternalFormat.Rgba32i;

        default:
          return SizedInternalFormat.Rgba32i;
      }
    }

    private static uint[] ConvertRgbaGridToUintArray_(IGrid<Color> rgbaGrid) {
      var (width, height) = (rgbaGrid.Width, rgbaGrid.Height);
      var uintArray = new uint[width * height];

      for (var y = 0; y < height; ++y) {
        for (var x = 0; x < width; ++x) {
          var i = y * width + x;
          var rgba = rgbaGrid[x, y];
          uintArray[i] = rgba.I;
        }
      }

      return uintArray;
    }

    private class TextureOpentk : DiscardableImpl, ITexture {
      private readonly uint textureId_;

      public TextureOpentk(uint textureId) {
        this.textureId_ = textureId;
        this.OnDiscard += _ => this.DestroyTexture_();
      }

      private void DestroyTexture_() {
        var textureIdArray = new[] {this.textureId_};
        GL.DeleteTextures(1, textureIdArray);
        this.Discard();
      }

      public IColor GetPixel(int x, int y) {
        return Color.FromRgba(0);
      }

      public IGrid<IColor> GetAllPixels() {
        throw new System.Exception();
      }
    }
  }
}