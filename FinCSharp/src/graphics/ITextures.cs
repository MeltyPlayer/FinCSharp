namespace fin.graphics {
  public interface ITextures {
    ITexture Create(ImageData imageData);
    IPixelBufferObject Create(int width, int height);
  }
}