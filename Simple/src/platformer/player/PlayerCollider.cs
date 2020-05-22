using System;

using fin.data.collections.grid;

using simple.platformer.player;
using simple.platformer.world;

using CMath = System.Math;
using Math = fin.math.Math;

namespace simple.platformer {
  public class PlayerCollider {
    private CachedBlocks cachedBlocks_ = new CachedBlocks();

    public PlayerStateMachine StateMachine { get; set; }
    public Rigidbody Rigidbody { get; set; }

    public void TickCollisions() {
      this.cachedBlocks_.CheckAll((this.CenterX, this.CenterY),
                                  this.CheckCollisions_);
    }

    private void CheckCollisions_(
        (int, int) relativeCacheIndex,
        (double, double) blockPosition) {
      var (relativeCacheC, relativeCacheR) = relativeCacheIndex;
      var isFloor =
          !this.cachedBlocks_.Check(relativeCacheC, relativeCacheR - 1);
      var isCeiling =
          !this.cachedBlocks_.Check(relativeCacheC, relativeCacheR + 1);
      var isLeftWall =
          !this.cachedBlocks_.Check(relativeCacheC - 1, relativeCacheR);
      var isRightWall =
          !this.cachedBlocks_.Check(relativeCacheC + 1, relativeCacheR);


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

      if (this.RightX > blockLeftX && this.LeftX < blockRightX) {
        if (Math.IsIncreasing(this.PreviousBottomY, blockY, this.BottomY)) {
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

      if (this.RightX > blockLeftX && this.LeftX < blockRightX) {
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
            this.LeftX,
            blockLeftX,
            this.RightX)) {
          this.RightX = blockLeftX;
          if (this.Rigidbody.XVelocity > 0) {
            this.Rigidbody.XVelocity = 0;
          }
        }
      }
    }

    private void PerformRightWallCollision_(double blockX, double blockY) {
      var blockSize = LevelConstants.SIZE;

      var blockRightX = blockX + blockSize;
      var blockTopY = blockY;
      var blockBottomY = blockY + blockSize;

      if (this.BottomY > blockTopY && this.TopY < blockBottomY) {
        if (Math.IsIncreasing(this.LeftX,
                              blockRightX,
                              this.RightX
                /*, this.PreviousLeftX*/)) {
          this.LeftX = blockRightX;
          if (this.Rigidbody.XVelocity < 0) {
            this.Rigidbody.XVelocity = 0;
          }
        }
      }
    }

    private double CenterX => this.X;
    private double CenterY => this.Y - PlayerConstants.VSIZE / 2;


    private double LeftX {
      get => this.X - PlayerConstants.HSIZE / 2;
      set => this.X = value + PlayerConstants.HSIZE / 2;
    }

    private double RightX {
      get => this.X + PlayerConstants.HSIZE / 2;
      set => this.X = value - PlayerConstants.HSIZE / 2;
    }

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


    private double X {
      get => this.Rigidbody.X;
      set => this.Rigidbody.X = value;
    }

    // TODO: Remove 480 references.
    // TODO: Remove flipping.
    private double Y {
      get => 480 - this.Rigidbody.Y;
      set => this.Rigidbody.Y = 480 - value;
    }

    private double PreviousX => this.Rigidbody.PreviousX;
    private double PreviousY => 480 - this.Rigidbody.PreviousY;

    private class CachedBlocks {
      private enum CacheState {
        UNDEFINED,
        EMPTY,
        FILLED,
      }

      private IFinGrid<CacheState> cachedBlocks_;

      public int Width { get; }
      public int Height { get; }

      private int centerLevelC_;
      private int centerLevelR_;

      private int cacheHStart_;
      private int cacheVStart_;

      public CachedBlocks() {
        var blockSize = LevelConstants.SIZE;

        var cacheWidth =
            (int) (1 + 1 + CMath.Ceiling(PlayerConstants.HSIZE / blockSize) +
                   1 +
                   1);
        var cacheHeight =
            (int) (1 + 1 + CMath.Ceiling(PlayerConstants.VSIZE / blockSize) +
                   1 +
                   1);

        this.cachedBlocks_ =
            new FinArrayGrid<CacheState>(cacheWidth,
                                         cacheHeight,
                                         CacheState.UNDEFINED);

        this.Width = this.cachedBlocks_.Width - 2;
        this.Height = this.cachedBlocks_.Height - 2;

        this.cacheHStart_ = (int) -CMath.Floor(this.Width / 2d);
        this.cacheVStart_ = (int) -CMath.Floor(this.Height / 2d);
      }

      public void CheckAll(
          (double, double) centerPosition,
          Action<(int, int), (double, double)> onCollision) {
        var blockSize = LevelConstants.SIZE;

        var (centerX, centerY) = centerPosition;
        (this.centerLevelC_, this.centerLevelR_) =
            ((int) CMath.Floor(centerX / blockSize),
             (int) CMath.Floor(centerY / blockSize));

        this.cachedBlocks_.Clear();

        for (var r = 0; r < this.Height; ++r) {
          for (var c = 0; c < this.Width; ++c) {
            var (relativeCacheC, relativeCacheR) =
                (this.cacheHStart_ + c, this.cacheVStart_ + r);

            if (this.Check(relativeCacheC, relativeCacheR)) {
              var levelPosition =
                  ((this.centerLevelC_ + relativeCacheC) * blockSize,
                   (this.centerLevelR_ + relativeCacheR) * blockSize);
              onCollision((relativeCacheC, relativeCacheR), levelPosition);
            }
          }
        }
      }

      public bool Check(int relativeCacheC, int relativeCacheR) {
        var levelC = this.centerLevelC_ + relativeCacheC;
        var levelR = this.centerLevelR_ + relativeCacheR;

        var absoluteCacheC = 1 + relativeCacheC - this.cacheHStart_;
        var absoluteCacheR = 1 + relativeCacheR - this.cacheVStart_;

        return this.CheckImpl_(absoluteCacheC, absoluteCacheR, levelC, levelR);
      }

      private bool CheckImpl_(
          int absoluteCacheC,
          int absoluteCacheR,
          int levelC,
          int levelR) {
        var state = this.cachedBlocks_[absoluteCacheC, absoluteCacheR];

        if (state == CacheState.FILLED) {
          return true;
        }
        if (state == CacheState.EMPTY) {
          return false;
        }

        var levelState = LevelConstants.LEVEL[levelC, levelR];
        this.cachedBlocks_[absoluteCacheC, absoluteCacheR] =
            levelState ? CacheState.FILLED : CacheState.EMPTY;
        return levelState;
      }
    }
  }
}