namespace fin.graphics.common {

  public abstract class IGraphics {
    public abstract IScreen Screen { get; }
    public abstract IPrimitives Primitives { get; }
    public abstract ITransform Transform { get; }
    public abstract ITextures Textures { get; }
    public abstract Render2d Render2d { get; }
  }
}