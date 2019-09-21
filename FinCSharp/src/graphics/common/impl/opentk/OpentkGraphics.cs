namespace fin.graphics.common.impl.opentk {
  public class OpenTkGraphics : IGraphics {
    public OpenTkGraphics() {
      this.R2d = new Render2d(this);
    }

    public override IScreen S { get; } = new ScreenOpentk();
    public override IPrimitives P { get; } = new OpenTkPrimitives();
    public override ITransform T { get; } = new TransformOpenTk();
    public override ITextures Ts { get; } = new TexturesOpentk();
    public override Render2d R2d { get; }
  }
}
