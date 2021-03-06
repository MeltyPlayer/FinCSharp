﻿namespace fin.graphics {
  public interface IGraphics {
    IScreen Screen { get; }
    IPrimitives Primitives { get; }
    ITransform Transform { get; }
    ITextures Textures { get; }
    Render2d Render2d { get; }
    Text Text { get; }
  }
}