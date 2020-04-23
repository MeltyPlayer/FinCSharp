using System;

using fin.graphics.text;

namespace fin.graphics {
  // TODO: Fix this.
  [Obsolete("Text is slow, VBOs/caching should be used instead.", false)]
  public class Text {
    private readonly IGraphics graphics_;
    private readonly IFontManager fontManager_;
    private IFont? font_;

    public Text(IGraphics graphics) {
      this.graphics_ = graphics;

      this.fontManager_ = new FontManager(graphics.Textures);
    }

    // TODO: Add support for more fonts and stuff.
    private IFont LoadFont_(string fontFileName) =>
        this.fontManager_.LoadFont(fontFileName);

    // TODO: Doubles or floats?
    // TODO: Rewrite this with models instead of on-the-fly primitives.
    public virtual Text Draw(
        float leftX,
        float topY,
        float fontWidth,
        float fontHeight,
        string text) {
      // TODO: Move this to a better place.
      if (this.font_ == null) {
        this.font_ = this.fontManager_.LoadFont("tahoma.ttf");
      }

      var primitives = this.graphics_.Primitives;
      var texture = this.font_.Texture;

      texture.Bind();
      primitives.Begin(PrimitiveType.QUADS);

      var lineHeight = (float) (this.font_.LineHeight * fontHeight);

      var currentX = leftX;
      var currentY = topY;
      foreach (char c in text) {
        if (c == '\n') {
          currentX = leftX;
          currentY += lineHeight;
          continue;
        }

        var glyph = this.font_.GetGlyph(c);

        var uvCoords = glyph.UvCoords;
        var currentLeftX = currentX;
        var currentRightX = (float) (currentLeftX + glyph.Width * fontWidth);
        var currentTopY =
            (float) (currentY + lineHeight - glyph.BearingY * fontHeight);
        var currentBottomY = (float) (currentTopY + glyph.Height * fontHeight);

        primitives.VertexUv(uvCoords[0]).Vertex(currentLeftX, currentTopY);
        primitives.VertexUv(uvCoords[1]).Vertex(currentRightX, currentTopY);
        primitives.VertexUv(uvCoords[2]).Vertex(currentRightX, currentBottomY);
        primitives.VertexUv(uvCoords[3]).Vertex(currentLeftX, currentBottomY);

        currentX += (float) (fontWidth * glyph.AdvanceX);
      }

      primitives.End();
      texture.Unbind();

      return this;
    }
  }
}