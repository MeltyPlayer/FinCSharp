using simple.platformer.player;
using simple.platformer.world;

using CMath = System.Math;
using FinMath = fin.math.FinMath;

namespace simple.platformer {
  public class PlayerCollider {
    public PlayerStateMachine StateMachine { get; set; }

    public PlayerRigidbody PlayerRigidbody { get; set; }
    private Rigidbody Rigidbody => this.PlayerRigidbody.Rigidbody;

    public void TickCollisions() {
      var levelGrid = LevelConstants.LEVEL_GRID;
      var blockSize = levelGrid.Size;

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

      for (var r = 0; r < windowHeight; ++r) {
        for (var c = 0; c < windowWidth; ++c) {
          var (relativeCacheC, relativeCacheR) =
              (relativeWindowHStart + c, relativeWindowVStart + r);
          var (absoluteLevelC, absoluteLevelR) = (centerLevelC + relativeCacheC,
                                                  centerLevelR +
                                                  relativeCacheR);
          var tile = levelGrid.GetAtIndex(absoluteLevelC, absoluteLevelR);
          if (tile != LevelTileTypes.EMPTY) {
            var blockPosition = (absoluteLevelC * blockSize,
                                 absoluteLevelR * blockSize);
            this.CheckCollisions_(tile, blockPosition);
          }
        }
      }
    }

    private void CheckCollisions_(
        LevelTileTypes tile,
        (float, float) blockPosition) {
      var isFloor = LevelGrid.Matches(tile, LevelTileTypes.FLOOR);
      var isCeiling = LevelGrid.Matches(tile, LevelTileTypes.CEILING);
      var isLeftWall = LevelGrid.Matches(tile, LevelTileTypes.LEFT_WALL);
      var isRightWall = LevelGrid.Matches(tile, LevelTileTypes.RIGHT_WALL);

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
      var collidedWithWall = false;
      if (isLeftWall && this.PerformLeftWallCollision_(blockX, blockY)) {
        collidedWithWall = true;
        this.StateMachine.WallSlidingOnLeft = false;
      }
      if (isRightWall && this.PerformRightWallCollision_(blockX, blockY)) {
        collidedWithWall = true;
        this.StateMachine.WallSlidingOnLeft = true;
      }

      if (collidedWithWall && this.StateMachine.IsInAir) {
        this.StateMachine.State = PlayerState.WALL_SLIDING;
      }

      // Last, we want to check the ceiling completely.
      if (isCeiling) {
        this.PerformCeilingCollision_(blockX, blockY);
      }
    }

    private void PerformFloorCollision_(float blockX, float blockY) {
      var blockSize = LevelConstants.SIZE;

      var blockLeftX = blockX;
      var blockRightX = blockX + blockSize;
      var blockTopY = blockY;

      if (this.PlayerRigidbody.RightX > blockLeftX &&
          this.PlayerRigidbody.LeftX < blockRightX) {
        if (FinMath.IsIncreasing(this.PlayerRigidbody.PreviousBottomY,
                              blockTopY,
                              this.PlayerRigidbody.BottomY)) {
          if (this.StateMachine.IsInAir) {
            this.StateMachine.State = PlayerState.LANDING;
            this.PlayerRigidbody.XVelocity *= .5f;
          }

          this.PlayerRigidbody.BottomY = blockTopY;
          this.Rigidbody.YVelocity = 0;
        }
      }
    }

    private void PerformCeilingCollision_(
        float blockX,
        float blockY,
        float wiggleRoom = 0) {
      var blockSize = LevelConstants.SIZE;

      var blockLeftX = blockX + wiggleRoom;
      var blockRightX = blockX + blockSize - wiggleRoom;
      var blockBottomY = blockY + blockSize;

      if (this.PlayerRigidbody.RightX > blockLeftX &&
          this.PlayerRigidbody.LeftX < blockRightX) {
        if ((this.PlayerRigidbody.PreviousTopY > blockBottomY &&
             this.PlayerRigidbody.TopY <= blockBottomY) ||
            (this.PlayerRigidbody.PreviousTopY >= blockBottomY &&
             this.PlayerRigidbody.TopY < blockBottomY)) {
          if (this.StateMachine.IsMovingUpwardInAirAndCanFall) {
            this.StateMachine.State = PlayerState.FALLING;
          }

          this.PlayerRigidbody.TopY = blockBottomY;
          this.Rigidbody.YVelocity = 0;
        }
      }
    }

    private bool PerformLeftWallCollision_(float blockX, float blockY) {
      var blockSize = LevelConstants.SIZE;

      var blockLeftX = blockX;
      var blockTopY = blockY;
      var blockBottomY = blockY + blockSize;

      if (this.PlayerRigidbody.BottomY > blockTopY &&
          this.PlayerRigidbody.TopY < blockBottomY) {
        if (FinMath.IsIncreasing( //this.PreviousRightX,
            this.PlayerRigidbody.LeftX,
            blockLeftX,
            this.PlayerRigidbody.RightX)) {
          this.PlayerRigidbody.RightX = blockLeftX;
          if (this.PlayerRigidbody.XVelocity > 0) {
            this.PlayerRigidbody.XVelocity = 0;
            return true;
          }
        }
      }

      return false;
    }

    private bool PerformRightWallCollision_(float blockX, float blockY) {
      var blockSize = LevelConstants.SIZE;

      var blockRightX = blockX + blockSize;
      var blockTopY = blockY;
      var blockBottomY = blockY + blockSize;

      if (this.PlayerRigidbody.BottomY > blockTopY &&
          this.PlayerRigidbody.TopY < blockBottomY) {
        if (FinMath.IsIncreasing(this.PlayerRigidbody.LeftX,
                              blockRightX,
                              this.PlayerRigidbody.RightX
            /*, this.PreviousLeftX*/)) {
          this.PlayerRigidbody.LeftX = blockRightX;
          if (this.PlayerRigidbody.XVelocity < 0) {
            this.PlayerRigidbody.XVelocity = 0;
            return true;
          }
        }
      }

      return false;
    }
  }
}