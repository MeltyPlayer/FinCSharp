using System;

using fin.data.collections.grid;

using simple.platformer.player;
using simple.platformer.world;

using CMath = System.Math;
using Math = fin.math.Math;

namespace simple.platformer {
  public class PlayerCollider {
    public PlayerStateMachine StateMachine { get; set; }

    public PlayerRigidbody PlayerRigidbody { get; set; }
    private Rigidbody Rigidbody => this.PlayerRigidbody.Rigidbody;

    public void TickCollisions() {
      var blockSize = LevelConstants.SIZE;

      var (centerLevelC, centerLevelR) =
          ((int) CMath.Floor(this.PlayerRigidbody.CenterX / blockSize),
           (int) CMath.Floor(this.PlayerRigidbody.CenterY / blockSize));

      var windowWidth =
          (int) (1 + CMath.Ceiling(PlayerConstants.HSIZE / blockSize) +
                 1);
      var windowHeight =
          (int) (1 + CMath.Ceiling(PlayerConstants.VSIZE / blockSize) +
                 1);

      var relativeWindowHStart = (int) -CMath.Floor(windowWidth / 2d);
      var relativeWindowVStart = (int) -CMath.Floor(windowHeight / 2d);

      var levelGrid = LevelConstants.LEVEL_GRID;
      for (var r = 0; r < windowHeight; ++r) {
        for (var c = 0; c < windowWidth; ++c) {
          var (relativeCacheC, relativeCacheR) =
              (relativeWindowHStart + c, relativeWindowVStart + r);
          var (absoluteLevelC, absoluteLevelR) = (centerLevelC + relativeCacheC,
                                                  centerLevelR +
                                                  relativeCacheR);
          var tile = levelGrid.GetAtIndex(absoluteLevelC, absoluteLevelR);
          if (tile != LevelTileType.EMPTY) {
            var blockPosition = (absoluteLevelC * blockSize,
                                 absoluteLevelR * blockSize);
            this.CheckCollisions_(tile, blockPosition);
          }
        }
      }
    }

    private void CheckCollisions_(
        LevelTileType tile,
        (double, double) blockPosition) {
      var isFloor = LevelGrid.Matches(tile, LevelTileType.FLOOR);
      var isCeiling = LevelGrid.Matches(tile, LevelTileType.CEILING);
      var isLeftWall = LevelGrid.Matches(tile, LevelTileType.LEFT_WALL);
      var isRightWall = LevelGrid.Matches(tile, LevelTileType.RIGHT_WALL);

      var (blockX, blockY) = blockPosition;
      // Landing on a floor has highest priority.
      if (isFloor) {
        this.PerformFloorCollision_(blockX, blockY);
      }

      // Hitting a ceiling has next highest priority, but only if it's in the middle.
      if (isCeiling) {
        this.PerformCeilingCollision_(blockX,
                                      blockY,
                                      PlayerConstants.HSIZE / 3);
      }

      // Hitting a wall or being close to the side of a wall should eject next.
      if (isLeftWall) {
        this.PerformLeftWallCollision_(blockX, blockY);
      }
      if (isRightWall) {
        this.PerformRightWallCollision_(blockX, blockY);
      }

      // Last, we want to check the ceiling completely.
      if (isCeiling) {
        this.PerformCeilingCollision_(blockX, blockY);
      }
    }

    private void PerformFloorCollision_(double blockX, double blockY) {
      var blockSize = LevelConstants.SIZE;

      var blockLeftX = blockX;
      var blockRightX = blockX + blockSize;
      var blockTopY = blockY;

      if (this.PlayerRigidbody.RightX > blockLeftX &&
          this.PlayerRigidbody.LeftX < blockRightX) {
        if (Math.IsIncreasing(this.PreviousBottomY, blockTopY, this.BottomY)) {
          if (this.StateMachine.IsInAir) {
            this.StateMachine.State = PlayerState.STANDING;
          }

          this.BottomY = blockTopY;
          this.Rigidbody.YVelocity = 0;
        }
      }
    }

    private void PerformCeilingCollision_(
        double blockX,
        double blockY,
        double wiggleRoom = 0) {
      var blockSize = LevelConstants.SIZE;

      var blockLeftX = blockX + wiggleRoom;
      var blockRightX = blockX + blockSize - wiggleRoom;
      var blockBottomY = blockY + blockSize;

      if (this.PlayerRigidbody.RightX > blockLeftX &&
          this.PlayerRigidbody.LeftX < blockRightX) {
        if ((this.PreviousTopY > blockBottomY &&
             this.TopY <= blockBottomY) ||
            (this.PreviousTopY >= blockBottomY &&
             this.TopY < blockBottomY)) {
          this.StateMachine.State = PlayerState.FALLING;

          this.TopY = blockBottomY;
          this.Rigidbody.YVelocity = 0;
        }
      }
    }

    private void PerformLeftWallCollision_(double blockX, double blockY) {
      var blockSize = LevelConstants.SIZE;

      var blockLeftX = blockX;
      var blockTopY = blockY;
      var blockBottomY = blockY + blockSize;

      if (this.BottomY > blockTopY && this.TopY < blockBottomY) {
        if (Math.IsIncreasing( //this.PreviousRightX,
            this.PlayerRigidbody.LeftX,
            blockLeftX,
            this.PlayerRigidbody.RightX)) {
          this.PlayerRigidbody.RightX = blockLeftX;
          if (this.PlayerRigidbody.XVelocity > 0) {
            this.PlayerRigidbody.XVelocity = 0;
          }
        }
      }
    }

    private void PerformRightWallCollision_(double blockX, double blockY) {
      var blockSize = LevelConstants.SIZE;

      var blockRightX = blockX + blockSize;
      var blockTopY = blockY;
      var blockBottomY = blockY + blockSize;

      if (this.BottomY > blockTopY &&
          this.TopY < blockBottomY) {
        if (Math.IsIncreasing(this.PlayerRigidbody.LeftX,
                              blockRightX,
                              this.PlayerRigidbody.RightX
            /*, this.PreviousLeftX*/)) {
          this.PlayerRigidbody.LeftX = blockRightX;
          if (this.PlayerRigidbody.XVelocity < 0) {
            this.PlayerRigidbody.XVelocity = 0;
          }
        }
      }
    }

    private double CenterY => this.Y - PlayerConstants.VSIZE / 2;

    private double BottomY {
      get => this.Y;
      set => this.Y = value;
    }

    private double TopY {
      get => this.Y - PlayerConstants.VSIZE;
      set => this.Y = value + PlayerConstants.VSIZE;
    }


    private double PreviousBottomY => this.PreviousY;
    private double PreviousTopY => this.PreviousY - PlayerConstants.VSIZE;

    // TODO: Remove 480 references.
    // TODO: Remove flipping.
    private double Y {
      get => this.Rigidbody.Y;
      set => this.Rigidbody.Y = value;
    }

    private double PreviousY => this.Rigidbody.PreviousY;
  }
}