namespace fin.graphics.common.impl.opentk {
  public class OpenTkGraphics : IGraphics {
    public OpenTkGraphics() {
      this.Render2d = new Render2d(this);
    }

    public override IScreen Screen { get; } = new ScreenOpentk();
    public override IPrimitives Primitives { get; } = new OpenTkPrimitives();
    public override ITransform Transform { get; } = new TransformOpenTk();
    public override ITextures Textures { get; } = new TexturesOpentk();
    public override Render2d Render2d { get; }
  }
}
