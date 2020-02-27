namespace fin.graphics.common.impl.opentk {
  public class GraphicsOpenTk : IGraphics {
    public GraphicsOpenTk() {
      this.Render2d = new Render2d(this);
    }

    public IScreen Screen { get; } = new ScreenOpentk();
    public IPrimitives Primitives { get; } = new OpenTkPrimitives();
    public ITransform Transform { get; } = new TransformOpenTk();
    public ITextures Textures { get; } = new TexturesOpentk();
    public Render2d Render2d { get; }
  }
}