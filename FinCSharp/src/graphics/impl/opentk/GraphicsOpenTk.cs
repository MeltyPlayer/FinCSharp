namespace fin.graphics.impl.opentk {
  public class GraphicsOpenTk : IGraphics {
    public GraphicsOpenTk() {
      this.Render2d = new Render2d(this);
    }

    public IScreen Screen { get; } = new ScreenOpenTk();
    public IPrimitives Primitives { get; } = new OpenTkPrimitives();
    public ITransform Transform { get; } = new TransformOpenTk();
    public ITextures Textures { get; } = new TexturesOpenTk();
    public Render2d Render2d { get; }
  }
}