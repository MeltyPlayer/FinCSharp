﻿using fin.app.phase;

namespace fin.graphics.common {

  public abstract class IGraphics {
    public abstract IScreen S { get; }
    public abstract IPrimitives P { get; }
    public abstract ITransform T { get; }
    public abstract ITextures Ts { get; }
    public abstract Render2d R2d { get; }
  }
}