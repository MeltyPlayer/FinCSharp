using System;

using fin.collision;
using fin.data.collections.list;
using fin.graphics;

namespace fin.gui {
  public interface IGuiFactory {
    ILayoutFactory Layout { get; }
    IContentFactory Content { get; }
  }


  public enum LengthUnitType {
    PIXELS,
  }

  public class Dimension {
    public LengthUnitType UnitType { get; set; }
    public float Length { get; set; }

    private Dimension(LengthUnitType unitType, float length) {
      this.UnitType = unitType;
      this.Length = length;
    }

    public static Dimension Pixels(float length)
      => new Dimension(LengthUnitType.PIXELS, length);
  }


  public interface ILayout {}

  public interface IBlock {
    IContent? Content { get; set; }
  }


  public interface ILayoutFactory {
    IAbsoluteLayout NewAbsolute();
  }

  public interface IAbsoluteLayout {
    IAbsoluteBlock Add(
        Dimension x,
        Dimension y,
        Dimension width,
        Dimension height,
        IContent? content = null);
  }

  public interface IAbsoluteBlock : IBlock {
    Dimension X { get; }
    Dimension Y { get; }

    Dimension Width { get; }
    Dimension Height { get; }
  }

  public interface IContentFactory {
    IPanel NewPanel(ILayout layout);
    IButton NewButton(Action handler);
    //IToggleButton NewToggleButton(Action<bool> handler);
  }

  public interface IContent {
    void Click(int x, int y);
    void Render(IGraphics g, int x, int y, int width, int height);
  }

  public interface IPanel : IContent {}

  public interface IButton : IContent {}

  public interface IToggleButton : IButton {
    bool IsToggled { get; }
  }
}