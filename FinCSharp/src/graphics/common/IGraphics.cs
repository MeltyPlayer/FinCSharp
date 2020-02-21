namespace fin.graphics.common {

  public interface IGraphics {
    IScreen Screen { get; }
    IPrimitives Primitives { get; }
    ITransform Transform { get; }
    ITextures Textures { get; }
    Render2d Render2d { get; }
  }
}