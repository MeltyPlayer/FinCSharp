using System;
using System.Collections.Generic;
using System.Text;

namespace simple.platformer.world {
  public static class LevelConstants {
    public static double SIZE = 32;

    public static LevelGrid LEVEL_GRID = new LevelGrid(LevelConstants.SIZE);
  }
}