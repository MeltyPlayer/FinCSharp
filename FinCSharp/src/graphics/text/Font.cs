namespace fin.graphics.text {
  using System;
  using System.Collections.Generic;
  using System.Collections.Immutable;
  using System.Linq;

  using color;

  using data.collections.grid;

  using io;

  using log;

  using SharpFont;

  using FinColor = color.FinColor;

  public interface IFontGlyph {
    // To calculate these stats, multiply by the font size.
    double Width { get; }
    double Height { get; }

    double AdvanceX { get; }
    double BearingY { get; }

    ImmutableArray<(double, double)> UvCoords { get; }
  }

  public interface IFont {
    ITexture Texture { get; }
    IFontGlyph GetGlyph(char c);
    double LineHeight { get; }
  }

  public interface IFontManager {
    IFont LoadFont(string fontFileName);
  }

  public class FontManager : IFontManager {
    private readonly ITextures textures_;

    public FontManager(ITextures textures) {
      this.textures_ = textures;
    }

    public IFont LoadFont(string fontFileName) {
      using var library = new Library();

      var bytes =
          LocalFileUtil.ReadBytes(
              LocalIo.Resources.GetFile("fonts/" + fontFileName));
      using var face = new Face(library, bytes, 0);

      const uint fontSize = 32;
      face.SetPixelSizes(0, fontSize);

      var supportedCharCodes = Enumerable.Range(32, 128);

      // Look up total width and max height to calculate atlas size.
      int atlasPadding = 2;
      int atlasWidth;
      int atlasHeight;
      int extraAtlasHeight;
      {
        int totalGlyphWidth = 0;
        int maxGlyphHeight = 0;

        foreach (var supportedCharCode in supportedCharCodes) {
          face.LoadChar((uint) supportedCharCode,
                        LoadFlags.Render | LoadFlags.Monochrome,
                        LoadTarget.Mono);

          var faceGlyph = face.Glyph;
          var bitmap = faceGlyph.Bitmap;

          totalGlyphWidth += bitmap.Width + atlasPadding;
          maxGlyphHeight = Math.Max(maxGlyphHeight, bitmap.Rows);
        }

        atlasWidth = (int) Math.Pow(2,
                                    Math.Ceiling(
                                        Math.Log(totalGlyphWidth) /
                                        Math.Log(2)));
        atlasHeight = (int) Math.Pow(2,
                                     Math.Ceiling(
                                         Math.Log(maxGlyphHeight) /
                                         Math.Log(2)));

        extraAtlasHeight = atlasHeight - maxGlyphHeight;
      }

      var fontGlyphs = new Dictionary<char, FontGlyph>();
      var atlasPixelGrid =
          new FinSparseGrid<FinColor>(atlasWidth,
                                   atlasHeight,
                                   ColorConstants.TRANSPARENT_BLACK);
      var atlasImageData = new ImageData(ImageType.RGBA, atlasPixelGrid);

      var x = 0;
      foreach (var supportedCharCode in supportedCharCodes) {
        face.LoadChar((uint) supportedCharCode,
                      LoadFlags.Default | LoadFlags.Render,
                      LoadTarget.Normal);

        // LoadChar() has loaded contents into Glyph.
        var faceGlyph = face.Glyph;

        // TODO: Is it possible to load this directly into the texture?
        var bitmap = faceGlyph.Bitmap;
        var width = bitmap.Width;
        var height = bitmap.Rows;

        byte[]? bufferData = null;
        try {
          bufferData = bitmap.BufferData;
        }
        catch (Exception e) {
          // TODO: This should probably not happen.
        }

        if (bufferData != null) {
          for (var r = 0; r < height; ++r) {
            for (var c = 0; c < width; ++c) {
              var i = r * width + c;
              var alpha = bufferData[i];
              var color = FinColor.FromRgbaB(255, 255, 255, alpha);
              atlasPixelGrid[x + c, r] = color;
            }
          }
        }

        var leftU = (1d * x) / atlasWidth;
        var rightU = (1d * (x + width)) / atlasWidth;
        var topV = 0d;
        var bottomV = (1d * height) / atlasHeight;
        var uvCoords = ImmutableArray.Create(new (double, double)[] {
            // Top-left
            (leftU, topV),

            // Top-right
            (rightU, topV),

            // Bottom-right
            (rightU, bottomV),

            // Bottom-left
            (leftU, bottomV)
        });

        var metrics = faceGlyph.Metrics;
        var fontGlyph = new FontGlyph((char) supportedCharCode,
                                      (1d * width) / fontSize,
                                      (1d * height) / fontSize,
                                      (1d * (int) metrics.HorizontalAdvance) /
                                      fontSize,
                                      (1d * (int) metrics.HorizontalBearingY +
                                       extraAtlasHeight) /
                                      fontSize,
                                      uvCoords);
        fontGlyphs[(char) supportedCharCode] = fontGlyph;

        x += width + atlasPadding;
      }

      var atlasTexture = this.textures_.Create(atlasImageData);
      return new Font(fontGlyphs,
                      atlasTexture,
                      // TODO: I'm getting sleepy, is this even right???
                      (1d * atlasHeight - extraAtlasHeight) / fontSize);
    }

    private class FontGlyph : IFontGlyph {
      private readonly char charCode_;
      public double Width { get; }

      public double Height { get; }

      public double AdvanceX { get; }

      public double BearingY { get; }

      public ImmutableArray<(double, double)> UvCoords { get; }

      public FontGlyph(
          char charCode,
          double width,
          double height,
          double advanceX,
          double bearingY,
          ImmutableArray<(double, double)> uvCoords) {
        this.charCode_ = charCode;
        this.Width = width;
        this.Height = height;
        this.AdvanceX = advanceX;
        this.BearingY = bearingY;
        this.UvCoords = uvCoords;
      }
    }

    private class Font : IFont {
      private readonly IDictionary<char, FontGlyph> glyphs_;
      public ITexture Texture { get; }
      public double LineHeight { get; }

      public Font(
          IDictionary<char, FontGlyph> glyphs,
          ITexture texture,
          double lineHeight) {
        this.glyphs_ = glyphs;
        this.Texture = texture;
        this.LineHeight = lineHeight;
      }

      public IFontGlyph GetGlyph(char charCode) => this.glyphs_[charCode];
    }
  }
}