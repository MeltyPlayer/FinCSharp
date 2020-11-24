namespace fin.graphics.impl.opentk {
  using System.Diagnostics;

  using data.collections.grid;

  using discardable;

  using color;

  using OpenTK.Graphics.OpenGL;

  public class TexturesOpenTk : ITextures {
    public ITexture Create(ImageData imageData) {
      var textureFormat =
          TexturesOpenTk.ConvertImageTypeToTextureFormat_(imageData.imageType);

      var (width, height) =
          (imageData.pixels.Width, imageData.pixels.Height);

      uint textureId;
      GL.GenTextures(1, out textureId);
      //GL.CreateTextures(TextureTarget.Texture2D, 1, out textureId);
      /*GL.TextureStorage2D(textureId,
                          1,
                          textureFormat,
                          width,
                          height);*/

      var pixelData = new uint[width * height];
      TexturesOpenTk.ConvertRgbaGridToUintArray_(
          imageData.pixels,
          ref pixelData,
          width,
          height);

      GL.BindTexture(TextureTarget.Texture2D, textureId);
      GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    width,
                    height,
                    0,
                    PixelFormat.AbgrExt,
                    PixelType.UnsignedByte,
                    pixelData);
      /*GL.TextureSubImage2D(textureId,
                           0,
                           0,
                           0,
                           width,
                           height,
                           PixelFormat.Rgba,
                           PixelType.Int,
                           pixelData);*/

      GL.TexParameter(TextureTarget.Texture2D,
                      TextureParameterName.TextureMinFilter,
                      (int) TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D,
                      TextureParameterName.TextureMagFilter,
                      (int) TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D,
                      TextureParameterName.TextureWrapS,
                      (int) TextureWrapMode.Clamp);
      GL.TexParameter(TextureTarget.Texture2D,
                      TextureParameterName.TextureWrapT,
                      (int) TextureWrapMode.Clamp);

      GL.BindTexture(TextureTarget.Texture2D, 0);

      return new TextureOpenTk(textureId);
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

    private static void ConvertRgbaGridToUintArray_(
        IFinGrid<FinColor> rgbaGrid,
        ref uint[] uintArray,
        int width,
        int height
    ) {
      for (var y = 0; y < height; ++y) {
        for (var x = 0; x < width; ++x) {
          var i = y * width + x;
          var rgba = rgbaGrid[x, y];
          uintArray[i] = rgba.I;
        }
      }
    }

    // TODO: Discard
    private class TextureOpenTk : ITexture {
      private readonly uint textureId_;

      public TextureOpenTk(uint textureId) {
        this.textureId_ = textureId;
      }

      // TODO: This is an internal detail that doesn't apply to all libraries, rethink this!
      public void Bind() =>
          GL.BindTexture(TextureTarget.Texture2D, this.textureId_);

      public void Unbind() =>
          GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public IPixelBufferObject Create(int width, int height)
      => new PixelBufferObjectOpenTk(width, height);

    // TODO: Width/height
    // TODO: Destroying
    // TODO: Pull these out into common methods
    // TODO: Configuration
    private class PixelBufferObjectOpenTk : IPixelBufferObject {
      private readonly uint bufferId_;
      private readonly uint textureId_;

      private readonly int width_;
      private readonly int height_;

      private uint[] pixelData_;

      public PixelBufferObjectOpenTk(int width, int height) {
        this.width_ = width;
        this.height_ = height;

        this.pixelData_ = new uint[width * height];

        //GL.GenBuffers(1, out this.bufferId_);
        GL.GenTextures(1, out this.textureId_);


        GL.BindTexture(TextureTarget.Texture2D, this.textureId_);

        GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMinFilter,
                        (int) TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMagFilter,
                        (int) TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureWrapS,
                        (int) TextureWrapMode.Clamp);
        GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureWrapT,
                        (int) TextureWrapMode.Clamp);

        GL.BindTexture(TextureTarget.Texture2D, 0);
      }

      public void SetAllPixels(IFinGrid<FinColor> rgbaGrid) {
        /*TexturesOpenTk.ConvertRgbaGridToUintArray_(
            rgbaGrid,
            ref this.pixelData_);

        GL.BindBuffer(BufferTarget.PixelPackBuffer, this.bufferId_);
        GL.ReadPixels(0,
                      0,
                      this.width_,
                      this.height_,
                      PixelFormat.AbgrExt,
                      PixelType.UnsignedByte,
                      this.pixelData_);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);*/

        TexturesOpenTk.ConvertRgbaGridToUintArray_(
            rgbaGrid,
            ref this.pixelData_,
            this.width_,
            this.height_);

        GL.BindTexture(TextureTarget.Texture2D, this.textureId_);
        //GL.BindTexture(BufferTarget.PixelUnpackBuffer, this.pixelData_);

        GL.TexImage2D(TextureTarget.Texture2D,
                      0,
                      PixelInternalFormat.Rgba,
                      this.width_,
                      this.height_,
                      0,
                      PixelFormat.AbgrExt,
                      PixelType.UnsignedByte,
                      this.pixelData_);

        GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMinFilter,
                        (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMagFilter,
                        (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureWrapS,
                        (int)TextureWrapMode.Clamp);
        GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureWrapT,
                        (int)TextureWrapMode.Clamp);

        //GL.BindTexture(BufferTarget.PixelUnpackBuffer, 0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
      }

      public void Bind() =>
          GL.BindTexture(TextureTarget.Texture2D, this.textureId_);

      public void Unbind() =>
          GL.BindTexture(TextureTarget.Texture2D, 0);
    }
  }
}